using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Text;

namespace rbkApiModules.Commons.Core;

public sealed class IntegrationEventPublisher : BackgroundService
{
    private static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IBrokerPublisher _publisher;
    private readonly IEventTypeRegistry _eventTypeRegistry;
    private readonly ILogger<IntegrationEventPublisher> _logger;
    private readonly IntegrationEventDispatcherOptions _options;

    public IntegrationEventPublisher(
        IServiceScopeFactory scopeFactory,
        IBrokerPublisher publisher,
        IEventTypeRegistry eventTypeRegistry,
        ILogger<IntegrationEventPublisher> logger,
        IOptions<IntegrationEventDispatcherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(publisher);
        ArgumentNullException.ThrowIfNull(eventTypeRegistry);
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        _scopeFactory = scopeFactory;
        _publisher = publisher;
        _eventTypeRegistry = eventTypeRegistry;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var loopStopwatch = Stopwatch.StartNew();

            try
            {
                Guid[] batch = [];

                var iterationId = GuidHelpers.CreateVersion7();

                using (var scope = _scopeFactory.CreateScope())
                {
                    var silentDbContext = _options.ResolveSilentDbContext(scope.ServiceProvider);

                    // QUIET "is there anything to do?" check (no logs, no telemetry)
                    if (!await HasDueIntegrationAsync(silentDbContext, cancellationToken))
                    {
                        await Task.Delay(AddJitter(TimeSpan.FromMilliseconds(_options.PollIntervalMs)), cancellationToken);
                        continue;
                    }
                }

                // If there is work to do, switch to a normal context, with logs and telemetry
                using (var scope = _scopeFactory.CreateScope())
                {
                    var messagingDbContext = _options.ResolveDbContext(scope.ServiceProvider);

                    var outerLogExtraData = new Dictionary<string, object>
                    {
                        ["IntegrationOutboxDispatcherInstanceId"] = iterationId.ToString("N"),
                    };
                    using var outerLogScope = _logger.BeginScope(outerLogExtraData);

                    var now = DateTime.UtcNow;
                    var claimTimeToLive = TimeSpan.FromMinutes(_options.ClaimDurationMin);

                    // Fetching candidate messages that are not processed yet, not claimed by another instance, and due for processing
                    var candidateIds = await messagingDbContext.IntegrationOutboxMessage
                        .Due(now, _options.MaxAttempts)
                        .OrderBy(x => x.CreatedUtc)
                        .Select(x => x.Id)
                        .Take(_options.BatchSize)
                        .ToListAsync(cancellationToken);

                    // In the case of multiple dispatchers processing messages, this check
                    // will avoid the next update state in the database
                    if (candidateIds.Count == 0)
                    {
                        await Task.Delay(AddJitter(TimeSpan.FromMilliseconds(_options.PollIntervalMs)), cancellationToken);
                        continue;
                    }

                    var claimedCount = await messagingDbContext.IntegrationOutboxMessage
                        .Due(now, _options.MaxAttempts)
                        .Where(x => candidateIds.Contains(x.Id))
                        .ExecuteUpdateAsync(s => s
                            .SetProperty(x => x.ClaimedBy, x => iterationId)
                            .SetProperty(x => x.ClaimedUntil, x => now.Add(claimTimeToLive)), cancellationToken);

                    if (claimedCount == 0)
                    {
                        // Lost the race to another instance, skip this iteration
                        continue;
                    }

                    batch = await messagingDbContext.IntegrationOutboxMessage
                        .ClaimedBy(iterationId, now)
                        .OrderBy(x => x.CreatedUtc)
                        .Select(x => x.Id)
                        .ToArrayAsync(cancellationToken);

                    _logger.LogInformation("Found {Count} integration events to publish", batch.Length);
                }

                foreach (var messageId in batch)
                {
                    _logger.LogDebug("Processing message {Id}", messageId);

                    var messageStopwatch = Stopwatch.StartNew();

                    using var processingScope = _scopeFactory.CreateScope();

                    var processingDbContext = _options.ResolveSilentDbContext(processingScope.ServiceProvider);

                    var message = await processingDbContext.IntegrationOutboxMessage
                        .Where(x => x.Id == messageId)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (message is null)
                    {
                        _logger.LogWarning("Message {Id} not found in the outbox, skipping", messageId);
                        continue;
                    }

                    if (message.ProcessedUtc is not null)
                    {
                        _logger.LogWarning("Message {Id} already processed at {ProcessedUtc}, skipping", message.Id, message.ProcessedUtc);
                        continue;
                    }

                    if (message.ClaimedBy != null && message.ClaimedBy != iterationId)
                    {
                        _logger.LogWarning("Message {Id} is claimed by another instance {ClaimedBy}, skipping", message.Id, message.ClaimedBy);
                        continue;
                    }

                    var hasUpstream = TelemetryUtils.TryBuildUpstreamActivityContext(message, out var upstreamContext);
                    var links = hasUpstream ? [new ActivityLink(upstreamContext)] : (IEnumerable<ActivityLink>?)null;

                    using var publisherActivity =
                        EventsTracing.ActivitySource.StartActivity(
                            "integration-outbox.publish",
                            ActivityKind.Producer,
                            default(ActivityContext), // no parent, otherwise it will grow forever until the next http request, use linked activities instead
                            null,
                            links,
                            default);

                    if (publisherActivity is not null)
                    {
                        publisherActivity.SetTag("messaging.system", "rabbitmq");
                        publisherActivity.SetTag("messaging.destination_kind", "topic");
                        publisherActivity.SetTag("messaging.message_id", message.Id);
                        publisherActivity.SetTag("messaging.event.name", message.Name);
                        publisherActivity.SetTag("messaging.event.version", message.Version);

                        // For UI's that don't support showing the linked activities, we need to manually
                        // query the other spans when debugging or investigating issues
                        // Adding upstream trace and span IDs as tags to help with that
                        if (hasUpstream)
                        {
                            publisherActivity.SetTag("upstream.trace_id", upstreamContext.TraceId.ToString());
                            publisherActivity.SetTag("upstream.span_id", upstreamContext.SpanId.ToString());
                        }

                        publisherActivity.SetTag("correlation.id", message.CorrelationId ?? "");
                    }

                    var logExtraData = new Dictionary<string, object>
                    {
                        ["EventId"] = message.Id,
                        ["CorrelationId"] = message.CorrelationId ?? string.Empty,
                        ["Name"] = message.Name,
                        ["Version"] = message.Version,
                        ["Username"] = message.Username,
                        ["TenantId"] = message.TenantId
                    };

                    using var scopeLog = _logger.BeginScope(logExtraData);

                    try
                    {
                        if (!_eventTypeRegistry.TryResolve(message.Name, message.Version, out var integrationEventType))
                        {
                            _logger.LogWarning("Unknown integration event type {Name} v{Version}. Marking message {MessageId} as poisoned.", message.Name, message.Version, message.Id);

                            message.MarkAsPoisoned();

                            await processingDbContext.SaveChangesAsync(cancellationToken);

                            EventsMeters.IntegrationOutbox_MessagesPoisoned.Add(1);

                            continue;
                        }

                        var envelopeType = typeof(EventEnvelope<>).MakeGenericType(integrationEventType);

                        // Not using the serialization result, this is just a validation step, if the payload is invalid, we throw
                        try
                        {
                            var envelope = JsonEventSerializer.Deserialize(message.Payload, envelopeType);

                            if (envelope is null)
                            {
                                throw new InvalidOperationException($"Deserialized envelope for {message.Name} v{message.Version} is null.");
                            }
                        }
                        catch (System.Text.Json.JsonException jsonException)
                        {
                            _logger.LogError(jsonException, "Invalid payload for integration event {EventId} ({Name} v{Version}). Marking message as poisoned.", message.Id, message.Name, message.Version);

                            message.MarkAsPoisoned();

                            await processingDbContext.SaveChangesAsync(cancellationToken);

                            EventsMeters.IntegrationOutbox_MessagesPoisoned.Add(1);

                            continue;
                        }

                        var topic = $"{message.Name}.v{message.Version}";

                        // Inject W3C trace headers
                        var headers = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

                        if (publisherActivity is not null)
                        {
                            Propagator.Inject(
                                new PropagationContext(publisherActivity.Context, Baggage.Current),
                                headers,
                                static (headers, key, value) => headers[key] = Encoding.UTF8.GetBytes(value));
                        }

                        // RabbitMQ specific headers
                        headers["message-id"] = message.Id.ToString("N");
                        headers["event-name"] = message.Name;
                        headers["event-version"] = message.Version.ToString();
                        headers["correlation-id"] = message.CorrelationId ?? string.Empty;

                        await _publisher.PublishAsync(topic, Encoding.UTF8.GetBytes(message.Payload), headers, cancellationToken);

                        message.MarkAsProcessed();

                        await processingDbContext.SaveChangesAsync(cancellationToken);

                        EventsMeters.IntegrationOutbox_MessagesProcessed.Add(1);
                        EventsMeters.IntegrationOutbox_DispatchDurationMs.Record(messageStopwatch.Elapsed.TotalMilliseconds);
                    }
                    catch (PermanentPublishException ex)
                    {
                        EventsMeters.IntegrationOutbox_MessagesFailed.Add(1);
                        EventsMeters.IntegrationOutbox_DispatchDurationMs.Record(messageStopwatch.Elapsed.TotalMilliseconds);

                        _logger.LogError(ex, "Permanent publish failure for integration event {EventId}. Marking as poisoned.", message.Id);

                        message.MarkAsPoisoned();
                        await processingDbContext.SaveChangesAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        EventsMeters.IntegrationOutbox_MessagesFailed.Add(1);
                        EventsMeters.IntegrationOutbox_DispatchDurationMs.Record(messageStopwatch.Elapsed.TotalMilliseconds);

                        _logger.LogError(ex, "Failed to publish integration event {EventId}", message.Id);

                        message.Backoff();
                        await processingDbContext.SaveChangesAsync(cancellationToken);
                    }
                    finally
                    {
                        messageStopwatch.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(IntegrationEventPublisher)} loop error");
            }

            loopStopwatch.Stop();

            EventsMeters.IntegrationOutbox_LoopDurationMs.Record(loopStopwatch.Elapsed.TotalMilliseconds);
        }
    }

    private static TimeSpan AddJitter(TimeSpan baseDelay, double jitterFactor = 0.2)
    {
        var extraMs = baseDelay.TotalMilliseconds * jitterFactor * Random.Shared.NextDouble();

        return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds + extraMs);
    }

    private Task<bool> HasDueIntegrationAsync(MessagingDbContext messagingContext, CancellationToken cancellationToken)
    {
        using var _ = SuppressInstrumentationScope.Begin();

        var now = DateTime.UtcNow;

        return messagingContext.IntegrationOutboxMessage
            .AsNoTracking()
            .Due(now, _options.MaxAttempts)
            .AnyAsync(cancellationToken);
    }
}

public static class IntegrationOutboxQueryExtensions
{
    public static IQueryable<IntegrationOutboxMessage> Due(this IQueryable<IntegrationOutboxMessage> query, DateTime now, int maxAttempts)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .Where(x => x.ProcessedUtc == null)
            .Where(x => !x.IsPoisoned)
            .Where(x => x.DoNotProcessBeforeUtc == null || x.DoNotProcessBeforeUtc <= now)
            .Where(x => x.ClaimedUntil == null || x.ClaimedUntil < now)
            .Where(x => x.Attempts < maxAttempts);
    }

    public static IQueryable<IntegrationOutboxMessage> ClaimedBy(this IQueryable<IntegrationOutboxMessage> query, Guid claimedBy, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .Where(x => x.ClaimedBy == claimedBy)
            .Where(x => x.ClaimedUntil != null && x.ClaimedUntil >= now);
    }

}
