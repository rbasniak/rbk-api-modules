using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Context.Propagation;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Commons.Core;

// TODO: Poison message strategy: Record last error and implement a max-attempts policy (e.g., move to DLQ or stop retrying after N attempts). Add columns like LastError, LastAttemptUtc, NextAttemptUtc, and skip if NextAttemptUtc > now.

public abstract class IntegrationConsumerBackgroundService : BackgroundService
{
    protected static readonly TextMapPropagator Propagator = Propagators.DefaultTextMapPropagator;

    private static readonly ConcurrentDictionary<(Type HandlerType, Type EventType), Func<object, object, CancellationToken, Task>> HandleInvokerCache = new();

    private readonly IBrokerSubscriber _subscriber;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventTypeRegistry _eventTypeRegistry;
    private readonly ILogger _logger;

    protected abstract IReadOnlyDictionary<string, string[]> Subscriptions { get; }

    protected IntegrationConsumerBackgroundService(
        IBrokerSubscriber subscriber,
        IServiceScopeFactory scopeFactory,
        IEventTypeRegistry eventTypeRegistry,
        ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(subscriber);
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(eventTypeRegistry);
        ArgumentNullException.ThrowIfNull(logger);

        _subscriber = subscriber;
        _scopeFactory = scopeFactory;
        _eventTypeRegistry = eventTypeRegistry;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var tasks = Subscriptions.Select(x => RunSubscriptionLoopAsync(x.Key, x.Value, cancellationToken));
        return Task.WhenAll(tasks);
    }

    private async Task RunSubscriptionLoopAsync(string queue, string[] topics, CancellationToken cancellationToken)
    {
        var backoff = TimeSpan.FromSeconds(1);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                // capture 'queue' into the callback so we can trace the right destination
                await _subscriber.SubscribeAsync(
                    queue,
                    topics,
                    (topic, payload, headers, ct) => HandleAsync(queue, topic, payload, headers, ct),
                    cancellationToken);

                backoff = TimeSpan.FromSeconds(1);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Subscription loop crashed for queue {Queue}. Retrying in {Delay}.", queue, backoff);
                try { await Task.Delay(backoff, cancellationToken); } catch (OperationCanceledException) { return; }
                var next = backoff.TotalSeconds * 2;
                backoff = TimeSpan.FromSeconds(next > 30 ? 30 : next);
            }
        }
    }

    private async Task HandleAsync(
        string queue,
        string topic,
        ReadOnlyMemory<byte> payload,
        IReadOnlyDictionary<string, object?> headers,
        CancellationToken cancellationToken)
    {
        try
        {
            var json = Encoding.UTF8.GetString(payload.Span);

            var envelopeHeader = JsonEventSerializer.DeserializeHeader(json);
            if (envelopeHeader is null)
            {
                _logger.LogWarning("Invalid envelope. Topic {Topic}", topic);
                return;
            }

            var parent = Propagator.Extract(default, headers, static (x, key) =>
            {
                if (x.TryGetValue(key, out var v) && v is byte[] b) return new[] { Encoding.UTF8.GetString(b) };
                if (x.TryGetValue(key, out var v2) && v2 is string s) return new[] { s };
                return Array.Empty<string>();
            });

            using var activity = EventsTracing.ActivitySource.StartActivity(
                "integration-inbox.consume",
                ActivityKind.Consumer,
                parent.ActivityContext);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.destination", queue);  
            activity?.SetTag("messaging.rabbitmq.routing_key", topic);
            activity?.SetTag("messaging.message_id", envelopeHeader.EventId);
            activity?.SetTag("messaging.event.name", envelopeHeader.Name);
            activity?.SetTag("messaging.event.version", envelopeHeader.Version);

            using var scope = _scopeFactory.CreateScope();
            var databaseContext = scope.ServiceProvider.GetRequiredService<MessagingDbContext>();

            if (!_eventTypeRegistry.TryResolve(envelopeHeader.Name, envelopeHeader.Version, out var clrType))
            {
                _logger.LogError("Unknown event {Name} v{Version}. Topic {Topic}", envelopeHeader.Name, envelopeHeader.Version, topic);
                return;
            }

            var envelopeType = typeof(EventEnvelope<>).MakeGenericType(clrType);
            var typedEnvelope = JsonEventSerializer.Deserialize(json, envelopeType)
                ?? throw new InvalidOperationException($"Cannot deserialize envelope for {envelopeHeader.Name} v{envelopeHeader.Version}");

            var handlerInterface = typeof(IIntegrationEventHandler<>).MakeGenericType(clrType);
            var handlers = scope.ServiceProvider.GetServices(handlerInterface).Cast<object>();

            foreach (var handler in handlers)
            {
                var handlerName = handler.GetType().FullName!;

                var inserted = await TryInsertInboxOnceAsync(
                    databaseContext,
                    envelopeHeader.EventId,
                    handlerName,
                    DateTimeOffset.UtcNow,
                    cancellationToken);

                if (!inserted)
                {
                    var alreadyProcessed = await databaseContext.InboxMessages
                        .AsNoTracking()
                        .Where(x => x.EventId == envelopeHeader.EventId && x.HandlerName == handlerName)
                        .Select(x => x.ProcessedUtc != null)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (alreadyProcessed)
                    {
                        continue;
                    }
                }

                await databaseContext.InboxMessages
                    .Where(x => x.EventId == envelopeHeader.EventId && x.HandlerName == handlerName)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, x => x.Attempts + 1), cancellationToken);

                // Warm delegate lookup/compile (per handler type + event type)
                var invoker = HandleInvokerCache.GetOrAdd(
                    (handler.GetType(), clrType),
                    static x => BuildHandleInvoker(x.HandlerType, x.EventType));

                await invoker(handler, typedEnvelope, cancellationToken);

                await databaseContext.InboxMessages
                    .Where(x => x.EventId == envelopeHeader.EventId && x.HandlerName == handlerName)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.ProcessedUtc, x => DateTimeOffset.UtcNow.UtcDateTime), cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception processing message for queue {Queue}, topic {Topic}.", queue, topic);
            throw;
        }
    }

    // Compiles a fast delegate: (object handler, object envelope, CancellationToken) => handler.HandleAsync((EventEnvelope<T>)envelope, cancellationToken)
    private static Func<object, object, CancellationToken, Task> BuildHandleInvoker(Type handlerType, Type eventType)
    {
        var envelopeType = typeof(EventEnvelope<>).MakeGenericType(eventType);

        var method = handlerType.GetMethod(
            "HandleAsync",
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            types: new[] { envelopeType, typeof(CancellationToken) },
            modifiers: null)
            ?? throw new InvalidOperationException($"HandleAsync(EventEnvelope<{eventType.Name}>, CancellationToken) not found on {handlerType.FullName}");

        var handlerParam = Expression.Parameter(typeof(object), "x");
        var envelopeParam = Expression.Parameter(typeof(object), "x");
        var cancellationTokenParam = Expression.Parameter(typeof(CancellationToken), "x");

        var castHandler = Expression.Convert(handlerParam, handlerType);
        var castEnvelope = Expression.Convert(envelopeParam, envelopeType);

        var call = Expression.Call(castHandler, method, castEnvelope, cancellationTokenParam);

        return Expression.Lambda<Func<object, object, CancellationToken, Task>>(call, handlerParam, envelopeParam, cancellationTokenParam).Compile();
    }

    private async Task<bool> TryInsertInboxOnceAsync(DbContext databaseContext, Guid eventId, string handlerName, DateTimeOffset receivedUtc, CancellationToken cancellationToken)
    {
        var names = DatabaseMetadata.For<InboxMessage>(databaseContext);
        var c = names.Columns;

        var sql = $@"
INSERT INTO {names.Table} ({c[nameof(InboxMessage.EventId)]}, {c[nameof(InboxMessage.HandlerName)]}, {c[nameof(InboxMessage.ReceivedUtc)]}, {c[nameof(InboxMessage.Attempts)]})
VALUES (@p0, @p1, @p2, 0)
ON CONFLICT ({c[nameof(InboxMessage.EventId)]}, {c[nameof(InboxMessage.HandlerName)]}) DO NOTHING";

        var rows = await databaseContext.Database.ExecuteSqlRawAsync(sql, new object[] { eventId, handlerName, receivedUtc }, cancellationToken);
        return rows > 0;
    }
}
