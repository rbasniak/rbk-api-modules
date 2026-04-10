// TODO: DONE, REVIEWED

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace rbkApiModules.Commons.Core;

public sealed class OutboxSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IRequestContext _requestContext;
    private readonly ILogger<OutboxSaveChangesInterceptor> _logger;
    public OutboxSaveChangesInterceptor(IRequestContext requestContext, ILogger<OutboxSaveChangesInterceptor> logger)
    {
        ArgumentNullException.ThrowIfNull(requestContext);
        ArgumentNullException.ThrowIfNull(logger);

        _requestContext = requestContext;
        _logger = logger;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context is not DbContext context)
        {
            return base.SavingChanges(eventData, result);
        }

        PersistDomainEventsToOutbox(context);
        
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not DbContext context)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        PersistDomainEventsToOutbox(context);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void PersistDomainEventsToOutbox(DbContext context)
    {
        // Usually called automatically by EF when SaveChanges is called but we call it explicitly here just in case
        context.ChangeTracker.DetectChanges();

        var aggregatesWithEvents = context.ChangeTracker.Entries<AggregateRoot>()
           .Where(x => x.Entity is AggregateRoot aggregateRoot && aggregateRoot.GetDomainEvents().Count > 0)
           .Select(x => (AggregateRoot)x.Entity)
           .ToArray();

        var activity = System.Diagnostics.Activity.Current;

        string? traceId = null;
        string? parentSpanId = null;
        int? traceFlags = null;
        string? traceState = null;

        if (activity is not null)
        {
            traceId = activity.Context.TraceId.ToString();
            parentSpanId = activity.Context.SpanId.ToString();
            traceFlags = (int)activity.Context.TraceFlags;
            traceState = activity.Context.TraceState;
        }

        var now = DateTime.UtcNow;

        foreach (var aggregate in aggregatesWithEvents)
        {
            var domainEvents = aggregate.GetDomainEvents();

            foreach (var domainEvent in domainEvents)
            {
                var envelope = EventEnvelopeFactory.Wrap(domainEvent, _requestContext.TenantId, _requestContext.Username, _requestContext.CorrelationId, _requestContext.CausationId);
                
                string payload;
                try
                {
                    payload = JsonEventSerializer.Serialize(envelope);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to serialize envelope {EventId} ({Name} v{Version})", envelope.EventId, envelope.Name, envelope.Version);

                    throw new InvalidOperationException(
                        $"Failed to serialize outbox envelope {envelope.EventId} ({envelope.Name} v{envelope.Version}). See inner exception for details.", ex);
                }

                var message = new DomainOutboxMessage
                {
                    Id = envelope.EventId,
                    Name = envelope.Name,
                    Version = envelope.Version,
                    TenantId = envelope.TenantId,
                    Username = envelope.Username,
                    OccurredUtc = envelope.OccurredUtc,
                    CorrelationId = envelope.CorrelationId,
                    CausationId = envelope.CausationId,
                    Payload = payload,
                    CreatedUtc = now,
                    TraceId = traceId,
                    ParentSpanId = parentSpanId,
                    TraceFlags = traceFlags,
                    TraceState = traceState
                };

                context.Set<DomainOutboxMessage>().Add(message);
            }

            aggregate.ClearDomainEvents();
        }
    }
} 