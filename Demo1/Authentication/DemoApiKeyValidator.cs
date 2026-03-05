using rbkApiModules.Identity.Core;

namespace Demo1.Authentication;

public class DemoApiKeyValidator : IApiKeyValidator
{
    public const string ValidApiKey = "demo-api-key-12345";

    public Task<bool> ValidateApiKey(string apiKey)
    {
        return Task.FromResult(string.Equals(apiKey, ValidApiKey, StringComparison.Ordinal));
    }
}
