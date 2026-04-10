using Microsoft.AspNetCore.Authentication;

namespace rbkApiModules.Identity.Core;

public interface IApiKeyValidator
{
    Task<AuthenticateResult> AuthenticateAsync(string apiKey, CancellationToken cancellationToken);
}
