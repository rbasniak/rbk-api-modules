// TODO: DONE, REVIEWED

namespace rbkApiModules.Commons.Core;

public interface IIntegrationOutbox
{
    Task<Guid> Enqueue<T>(EventEnvelope<T> envelope, CancellationToken cancellationToken);
}
