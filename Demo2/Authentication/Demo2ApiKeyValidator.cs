using rbkApiModules.Identity.Core;

namespace Demo2.Authentication;

public class Demo2ApiKeyValidator : IApiKeyValidator
{
    public const string ValidApiKey = "demo-api-key-12345";

    public Task<bool> ValidateApiKey(string apiKey)
    {
        return Task.FromResult(string.Equals(apiKey, ValidApiKey, StringComparison.Ordinal));
    }
}
