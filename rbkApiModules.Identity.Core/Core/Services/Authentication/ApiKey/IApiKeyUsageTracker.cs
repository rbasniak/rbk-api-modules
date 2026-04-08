namespace rbkApiModules.Identity.Core;

public interface IApiKeyUsageTracker
{
    Task RecordSuccessfulAuthenticationAsync(Guid apiKeyId, CancellationToken cancellationToken);
}
