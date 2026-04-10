namespace rbkApiModules.Identity.Core;

public interface IApiKeyAuthenticationCacheInvalidation
{
    void InvalidateByKeyHash(string keyHash);
}
