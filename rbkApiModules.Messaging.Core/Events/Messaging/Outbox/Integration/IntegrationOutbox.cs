// TODO: DONE, REVIEWED

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Default implementation of <see cref="IIntegrationOutbox"/> which stores
/// integration events in the database.
/// </summary>
public class IntegrationOutbox : IIntegrationOutbox
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly DomainEventDispatcherOptions _options;

    public IntegrationOutbox(IServiceScopeFactory scopeFactory, IOptions<DomainEventDispatcherOptions> options)
    {
        ArgumentNullException.ThrowIfNull(scopeFactory);
        ArgumentNullException.ThrowIfNull(options);
        
        _scopeFactory = scopeFactory;
        _options = options.Value;
    }

    public async Task<Guid> Enqueue<T>(EventEnvelope<T> envelope, CancellationToken cancellationToken)
    {
        var activity = System.Diagnostics.Activity.Current?.Context ?? default;

        using var scope = _scopeFactory.CreateScope();

        var messagingDbContext = _options.ResolveSilentDbContext!(scope.ServiceProvider);

        var message = new IntegrationOutboxMessage
        {
            Id = envelope.EventId,
            Name = envelope.Name,
            Version = envelope.Version,
            TenantId = envelope.TenantId,
            Username = envelope.Username,
            OccurredUtc = envelope.OccurredUtc,
            Payload = JsonEventSerializer.Serialize(envelope),
            CreatedUtc = DateTime.UtcNow,
            TraceId = activity.TraceId.ToString(),
            ParentSpanId = activity.SpanId.ToString(),
            TraceFlags = (int)activity.TraceFlags,
            TraceState = activity.TraceState,
            CorrelationId = envelope.CorrelationId,
            CausationId = envelope.CausationId,
        };

        messagingDbContext.Add(message);

        await messagingDbContext.SaveChangesAsync(cancellationToken);
        
        return message.Id;
    }
}
