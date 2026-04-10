using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public sealed class ApiKeyLastUsedThrottler : IApiKeyLastUsedThrottler
{
    private readonly DbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _minInterval;

    public ApiKeyLastUsedThrottler(
        IEnumerable<DbContext> contexts,
        IMemoryCache memoryCache,
        IOptions<RbkAuthenticationOptions> authOptions)
    {
        _context = contexts.GetDefaultContext();
        _memoryCache = memoryCache;
        _minInterval = authOptions.Value._builtInApiKeyOptions.LastUsedUpdateMinInterval;
    }

    public async Task TouchIfDueAsync(Guid apiKeyId, CancellationToken cancellationToken)
    {
        var throttleKey = $"rbk:apikey:lastused-throttle:{apiKeyId}";

        if (_memoryCache.TryGetValue(throttleKey, out _))
        {
            return;
        }

        _memoryCache.Set(throttleKey, 1, _minInterval);

        var entity = await _context.Set<ApiKey>().FindAsync(new object[] { apiKeyId }, cancellationToken);
        if (entity == null)
        {
            return;
        }

        entity.MarkUsed();
        await _context.SaveChangesAsync(cancellationToken);
    }
}
