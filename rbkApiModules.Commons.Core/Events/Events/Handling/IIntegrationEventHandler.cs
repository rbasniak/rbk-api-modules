// TODO: DONE, REVIEWED

namespace rbkApiModules.Commons.Core;

/// <summary>
/// Contract implemented by services that handle integration events dispatched
/// from the integration outbox.
/// </summary>
public interface IIntegrationEventHandler<TIntegrationEvent>
{
    Task HandleAsync(EventEnvelope<TIntegrationEvent> envelope, CancellationToken cancellationToken);
}
