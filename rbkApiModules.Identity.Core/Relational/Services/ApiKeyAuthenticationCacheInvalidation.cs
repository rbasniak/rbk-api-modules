using Microsoft.Extensions.Caching.Memory;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public sealed class ApiKeyAuthenticationCacheInvalidation : IApiKeyAuthenticationCacheInvalidation
{
    private readonly IMemoryCache _memoryCache;

    public ApiKeyAuthenticationCacheInvalidation(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void InvalidateByKeyHash(string keyHash)
    {
        if (string.IsNullOrEmpty(keyHash))
        {
            return;
        }

        _memoryCache.Remove(ApiKeyCacheKeys.AuthenticationEntry(keyHash));
    }
}
