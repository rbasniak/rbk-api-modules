// TODO: DONE, REVIEWED

// DOCS: Keep handlers DB-only. No HTTP, queues, or other databases inside the transaction.
//       If a handler must trigger external effects, persist an outbox record for that effect instead of calling the external system inline.
//       Watch transaction length and lock contention. Consider smaller batches, reasonable command timeout, and clear guidance that handlers must be fast and idempotent.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace rbkApiModules.Commons.Core;

public sealed class DomainEventDispatcher : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventTypeRegistry _eventTypeRegistry;
    private readonly ILogger<DomainEventDispatcher> _logger;
    private readonly DomainEventDispatcherOptions _options;

    private static readonly ConcurrentDictionary<(Type HandlerType, Type EnvelopeType), Func<object, object, CancellationToken, Task>> _dispatchers = [];

    public DomainEventDispatcher(
        IServiceScopeFactory scopeFactory,
        IEventTypeRegistry registry,
        ILogger<DomainEventDispatcher> logger,
        IOptions<DomainEventDispatcherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(registry);
        ArgumentNullException.ThrowIfNull(logger);

        _scopeFactory = scopeFactory;
        _eventTypeRegistry = registry;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("OutboxDispatcher started");

        while (!cancellationToken.IsCancellationRequested)
        {
            var loopStopwatch = Stopwatch.StartNew();

            try
            {
                Guid[] batch = [];
                
                var iterationId = GuidHelpers.CreateVersion7();

                // QUIET "is there anything to do?" check (no logs, no telemetry)
                using (var scope = _scopeFactory.CreateScope())
                {
                    var silentDbContext = _options.ResolveSilentDbContext(scope.ServiceProvider);

                    if (!await HasDueEventsAsync(silentDbContext, cancellationToken))
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
                        ["DomainOutboxDispatcherInstanceId"] = iterationId.ToString("N"),
                    };

                    var now = DateTime.UtcNow;
                    var claimTimeToLive = TimeSpan.FromMinutes(_options.ClaimDurationMin);

                    // Fetching candidate messages that are not processed yet, not claimed by another instance, and due for processing
                    var candidateIds = await messagingDbContext.DomainOutboxMessage
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

                    var claimedCount = await messagingDbContext.DomainOutboxMessage
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

                    batch = await messagingDbContext.DomainOutboxMessage
                        .ClaimedBy(iterationId, now)
                        .OrderBy(x => x.CreatedUtc)
                        .Select(x => x.Id)
                        .ToArrayAsync(cancellationToken);

                    _logger.LogInformation("Found {Count} domain messages to dispatch", batch.Length);
                }

                // Process each message in its own scope/transaction
                foreach (var messageId in batch)
                {
                    _logger.LogDebug("Processing message {Id}", messageId);

                    var messageStopwatch = Stopwatch.StartNew();

                    using var processingScope = _scopeFactory.CreateScope();

                    var processingDbContext = _options.ResolveDbContext(processingScope.ServiceProvider);

                    var message = await processingDbContext.DomainOutboxMessage
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

                    using var dispatcherActivity =
                        EventsTracing.ActivitySource.StartActivity(
                            "domain-outbox.dispatch",
                            ActivityKind.Consumer,
                            default(ActivityContext), // no parent, otherwise it will grow forever until the next http request, use linked activities instead
                            null,
                            links,
                            default);

                    if (dispatcherActivity is not null)
                    {
                        dispatcherActivity.SetTag("messaging.system", "outbox");
                        dispatcherActivity.SetTag("messaging.domain-event.id", message.Id);
                        dispatcherActivity.SetTag("messaging.domain-event.name", message.Name);
                        dispatcherActivity.SetTag("messaging.domain-event.version", message.Version);

                        // For UI's that don't support showing the linked activities, we need to manually
                        // query the other spans when debugging or investigating issues
                        // Adding upstream trace and span IDs as tags to help with that
                        if (hasUpstream)
                        {
                            dispatcherActivity.SetTag("upstream.trace_id", upstreamContext.TraceId.ToString());
                            dispatcherActivity.SetTag("upstream.span_id", upstreamContext.SpanId.ToString());
                        }

                        dispatcherActivity.SetTag("correlation.id", message.CorrelationId ?? "");
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

                    using var messageScopedLog = _logger.BeginScope(logExtraData);

                    await using var transaction = await processingDbContext.Database.BeginTransactionAsync(cancellationToken);

                    try
                    {
                        if (!_eventTypeRegistry.TryResolve(message.Name, message.Version, out var domainEventType))
                        {
                            _logger.LogWarning("Unknown domain event type {Name} v{Version}. Marking message {MessageId} as poisoned.", message.Name, message.Version, message.Id);

                            message.MarkAsPoisoned();

                            await processingDbContext.SaveChangesAsync(cancellationToken);

                            await transaction.CommitAsync(cancellationToken);

                            EventsMeters.DomainOutbox_MessagesPoisoned.Add(1);

                            continue;
                        }

                        var envelopeType = typeof(EventEnvelope<>).MakeGenericType(domainEventType);

                        object envelope;
                        try
                        {
                            envelope = JsonEventSerializer.Deserialize(message.Payload, envelopeType);

                            if (envelope is null)
                            {
                                throw new InvalidOperationException($"Deserialized envelope for {message.Name} v{message.Version} is null.");
                            }
                        }
                        catch (System.Text.Json.JsonException jsonException)
                        {
                            _logger.LogError(jsonException, "Invalid payload for domain event {EventId} ({Name} v{Version}). Marking message as poisoned.", message.Id, message.Name, message.Version);

                            message.MarkAsPoisoned();

                            await processingDbContext.SaveChangesAsync(cancellationToken);

                            await transaction.CommitAsync(cancellationToken);

                            EventsMeters.DomainOutbox_MessagesPoisoned.Add(1);

                            continue;
                        }

                        var handlers = ResolveHandlers(processingScope.ServiceProvider, domainEventType);

                        foreach (var handler in handlers)
                        {
                            var handlerName = handler.GetType().FullName!;

                            // Check if this handler has already processed the message at some point
                            var processedMessage = await processingDbContext.InboxMessages.FindAsync([message.Id, handlerName], cancellationToken);

                            // Just in case, but should not happen because if one handler fails, we fail all together
                            if (processedMessage is not null)
                            {
                                dispatcherActivity?.AddEvent(new ActivityEvent("handler.skipped",
                                   tags: new ActivityTagsCollection {
                                           { "handler", handlerName },
                                           { "reason", "inbox-duplicate" }
                                   }));

                                continue;
                            }

                            _logger.LogInformation("Dispatching {Name} v{Version} to {Handler}", message.Name, message.Version, handlerName);

                            using var handlerActivity = EventsTracing.ActivitySource.StartActivity(
                                "domain-event.handler", ActivityKind.Internal, dispatcherActivity?.Context ?? default);

                            handlerActivity?.SetTag("messaging.domain-event.handler", handlerName);
                            handlerActivity?.SetTag("messaging.domain-event.name", message.Name);
                            handlerActivity?.SetTag("messaging.domain-event.version", message.Version);

                            await InvokeHandler(handler, envelope, cancellationToken);

                            processingDbContext.InboxMessages.Add(new InboxMessage
                            {
                                EventId = message.Id,
                                HandlerName = handlerName,
                                ReceivedUtc = DateTime.UtcNow,
                                ProcessedUtc = DateTime.UtcNow,
                                Attempts = message.Attempts + 1 // +1 because the value in the original message will be updated only down the stream, when it is marked as processed or backed of
                            });
                        }

                        // mark as processed only after all handlers succeed
                        message.MarAsProcessed();

                        await processingDbContext.SaveChangesAsync(cancellationToken);

                        await transaction.CommitAsync(cancellationToken);

                        EventsMeters.DomainOutbox_MessagesProcessed.Add(1);
                        EventsMeters.DomainOutbox_DispatchDurationMs.Record(messageStopwatch.Elapsed.TotalMilliseconds);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Outbox dispatch failed for {Id}", message.Id);

                        // rollback transacation and backoff
                        try
                        {
                            await transaction.RollbackAsync(cancellationToken);
                        }
                        catch (Exception transactionException)
                        {
                            _logger.LogWarning(transactionException, "Failed to rollback transaction for message {Id}", message.Id);
                        }

                        dispatcherActivity?.AddEvent(new ActivityEvent("handler.error", tags: new ActivityTagsCollection
                            {
                                { "exception.message", ex.Message },
                                { "exception.details", ex.ToBetterString() },
                            }));

                        message.Backoff();

                        await processingDbContext.SaveChangesAsync(cancellationToken);

                        EventsMeters.DomainOutbox_MessagesFailed.Add(1);
                        EventsMeters.DomainOutbox_DispatchDurationMs.Record(messageStopwatch.Elapsed.TotalMilliseconds);
                    }
                    finally
                    {
                        messageStopwatch.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DomainEventDispatcher)} loop error");
            }

            loopStopwatch.Stop();

            EventsMeters.DomainOutbox_LoopDurationMs.Record(loopStopwatch.Elapsed.TotalMilliseconds);
        }

        _logger.LogInformation("OutboxDispatcher stopped");
    }

    private static TimeSpan AddJitter(TimeSpan baseDelay, double jitterFactor = 0.2)
    {
        var extraMs = baseDelay.TotalMilliseconds * jitterFactor * Random.Shared.NextDouble();

        return TimeSpan.FromMilliseconds(baseDelay.TotalMilliseconds + extraMs);
    }

    private IEnumerable<object> ResolveHandlers(IServiceProvider sp, Type clrType)
        => sp.GetServices(typeof(IDomainEventHandler<>).MakeGenericType(clrType))?.Cast<object>() ?? Array.Empty<object>();

    private static Task InvokeHandler(object handler, object envelope, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentNullException.ThrowIfNull(envelope);

        var handlerType = handler.GetType();
        var key = (HandlerType: handlerType, EnvelopeType: envelope.GetType());
        var invoker = _dispatchers.GetOrAdd(key, x =>
        {
            var methods = x.HandlerType.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.Name == nameof(IDomainEventHandler<object>.HandleAsync))
                .ToArray();

            var method = methods.FirstOrDefault(x => x.GetParameters().First().ParameterType == key.EnvelopeType);

            if (method is null)
            {
                throw new InvalidOperationException($"Handler {handler.GetType().FullName} does not have a public HandleAsync method.");
            }

            return (handler, envelope, cancellationToken) => (Task)method.Invoke(handler, [envelope, cancellationToken])!;
        });

        return invoker(handler, envelope, cancellationToken);
    }


    private static async Task<Guid[]> GetMessagesToProcessAsync(MessagingDbContext context, int maxAttempts, int batchSize, Guid iterationId, TimeSpan claimTimeToLive, CancellationToken cancellationToken)
    {
        using var _ = SuppressInstrumentationScope.Begin(); // no telemetry, avoid flooding it with idle loops data

        var now = DateTime.UtcNow; // close enough for "due" check

        var candidateIds = await context.Set<DomainOutboxMessage>()
            .AsNoTracking()
            .Due(now, maxAttempts)
            .NotClaimed(now)
            .OrderBy(x => x.CreatedUtc)
            .Select(x => x.Id) // get keys only, because messages are reloaded in the processing scope
            .Take(batchSize)
            .ToArrayAsync(cancellationToken);

        if (candidateIds.Length == 0)
        {
            return Array.Empty<Guid>();
        }

        var claimedCount = await context.Set<DomainOutboxMessage>()
            .Where(x => candidateIds.Contains(x.Id))
            .Due(now, maxAttempts)
            .NotClaimed(now)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.ClaimedBy, x => iterationId)
                .SetProperty(x => x.ClaimedUntil, x => now.Add(claimTimeToLive)), cancellationToken);

        if (claimedCount == 0)
        {
            return Array.Empty<Guid>();
        }

        var claimedIds = await context.Set<DomainOutboxMessage>()
            .AsNoTracking()
            .ClaimedBy(iterationId, now)
            .OrderBy(x => x.CreatedUtc)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        return claimedIds;
    }

    private Task<bool> HasDueEventsAsync(MessagingDbContext messagingDbContext, CancellationToken cancellationToken)
    {
        using var _ = SuppressInstrumentationScope.Begin();

        var now = DateTime.UtcNow;

        return messagingDbContext.DomainOutboxMessage
            .AsNoTracking()
            .Due(now, _options.MaxAttempts)
            .AnyAsync(cancellationToken);
    }
}

internal static class DomainOutboxQueryExtensions
{
    public static IQueryable<DomainOutboxMessage> Due(this IQueryable<DomainOutboxMessage> query, DateTime now, int maxAttempts)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .Where(x => x.ProcessedUtc == null)
            .Where(x => !x.IsPoisoned)
            .Where(x => x.DoNotProcessBeforeUtc == null || x.DoNotProcessBeforeUtc <= now)
            .Where(x => x.ClaimedUntil == null || x.ClaimedUntil < now)
            .Where(x => x.Attempts < maxAttempts);
    }

    public static IQueryable<DomainOutboxMessage> NotClaimed(this IQueryable<DomainOutboxMessage> query, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .Where(x => x.ClaimedUntil == null || x.ClaimedUntil < now);
    }

    public static IQueryable<DomainOutboxMessage> ClaimedBy(this IQueryable<DomainOutboxMessage> query, Guid claimedBy, DateTime now)
    {
        ArgumentNullException.ThrowIfNull(query);

        return query
            .Where(x => x.ClaimedBy == claimedBy)
            .Where(x => x.ClaimedUntil != null && x.ClaimedUntil >= now);
    }
}
