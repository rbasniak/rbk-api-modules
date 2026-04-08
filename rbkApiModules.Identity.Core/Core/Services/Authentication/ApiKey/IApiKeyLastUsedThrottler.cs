namespace rbkApiModules.Identity.Core;

public interface IApiKeyLastUsedThrottler
{
    Task TouchIfDueAsync(Guid apiKeyId, CancellationToken cancellationToken);
}
