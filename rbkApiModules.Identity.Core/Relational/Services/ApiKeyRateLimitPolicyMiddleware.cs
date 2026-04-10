using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public sealed class ApiKeyRateLimitPolicyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyRateLimitPolicyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(RbkAuthenticationSchemes.API_KEY, out var values))
        {
            await _next(context);
            return;
        }

        var raw = values.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(raw))
        {
            await _next(context);
            return;
        }

        var keyHash = ApiKeyMaterial.HashRawKey(raw);
        var memoryCache = context.RequestServices.GetRequiredService<IMemoryCache>();
        var cacheKey = ApiKeyCacheKeys.RateLimitPolicy(keyHash);

        if (!memoryCache.TryGetValue(cacheKey, out ApiKeyRateLimitPolicy _))
        {
            var authOptions = context.RequestServices.GetRequiredService<RbkAuthenticationOptions>();
            var defaultRpm = authOptions._builtInApiKeyOptions.RequestsPerMinute;
            var defaultBurst = defaultRpm;
            var cacheTtl = authOptions._builtInApiKeyOptions.CacheAbsoluteExpiration;

            await using var scope = context.RequestServices.CreateAsyncScope();
            var db = scope.ServiceProvider.GetServices<DbContext>().GetDefaultContext();

            var row = await db.Set<ApiKey>()
                .AsNoTracking()
                .Where(x => x.KeyHash == keyHash)
                .Select(x => new { x.IsActive, x.ExpirationDate, x.RequestsPerMinute, x.BurstLimit })
                .FirstOrDefaultAsync(context.RequestAborted)
                .ConfigureAwait(false);

            ApiKeyRateLimitPolicy policy;
            var now = DateTime.UtcNow;
            if (row == null || !row.IsActive || (row.ExpirationDate.HasValue && row.ExpirationDate.Value < now))
            {
                policy = new ApiKeyRateLimitPolicy(defaultRpm, defaultBurst);
            }
            else
            {
                policy = new ApiKeyRateLimitPolicy(row.RequestsPerMinute, row.BurstLimit);
            }

            memoryCache.Set(cacheKey, policy, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheTtl
            });
        }

        await _next(context);
    }
}
