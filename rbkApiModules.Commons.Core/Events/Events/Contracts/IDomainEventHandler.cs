namespace rbkApiModules.Commons.Core;

public interface IDomainEventHandler<TEvent>
{
    Task HandleAsync(EventEnvelope<TEvent> envelope, CancellationToken cancellationToken);
} 