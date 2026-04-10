using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public sealed class RbkRelationalApiKeyValidator : IApiKeyValidator
{
    private readonly DbContext _context;
    private readonly IMemoryCache _memoryCache;
    private readonly TimeSpan _cacheExpiration;
    private readonly IApiKeyUsageTracker _usageTracker;
    private readonly IApiKeyLastUsedThrottler _lastUsedThrottler;

    public RbkRelationalApiKeyValidator(
        IEnumerable<DbContext> contexts,
        IMemoryCache memoryCache,
        IOptions<RbkAuthenticationOptions> authOptions,
        IApiKeyUsageTracker usageTracker,
        IApiKeyLastUsedThrottler lastUsedThrottler)
    {
        _context = contexts.GetDefaultContext();
        _memoryCache = memoryCache;
        _cacheExpiration = authOptions.Value._builtInApiKeyOptions.CacheAbsoluteExpiration;
        _usageTracker = usageTracker;
        _lastUsedThrottler = lastUsedThrottler;
    }

    public async Task<AuthenticateResult> AuthenticateAsync(string apiKey, CancellationToken cancellationToken)
    {
        if (!ApiKeyMaterial.TryParseStoredPrefix(apiKey, ApiKeyMaterial.DefaultPublicPrefix, out var formatError))
        {
            return AuthenticateResult.Fail(formatError);
        }

        var keyHash = ApiKeyMaterial.HashRawKey(apiKey);
        var cacheKey = ApiKeyCacheKeys.AuthenticationEntry(keyHash);

        if (!_memoryCache.TryGetValue(cacheKey, out ApiKeyAuthCacheEntry entry))
        {
            var entity = await _context.Set<ApiKey>()
                .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.KeyHash == keyHash, cancellationToken);

            if (entity == null || !entity.IsActive)
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            if (entity.ExpirationDate.HasValue && entity.ExpirationDate.Value < DateTime.UtcNow)
            {
                return AuthenticateResult.Fail("API Key expired");
            }

            var accessClaims = entity.GetAccessClaims();

            foreach (var claim in accessClaims)
            {
                if (!claim.AllowApiKeyUsage)
                {
                    return AuthenticateResult.Fail("API Key uses a claim that is not allowed for API keys");
                }
            }

            var identifications = accessClaims.Select(x => x.Identification).ToList();

            entry = new ApiKeyAuthCacheEntry
            {
                Id = entity.Id,
                Name = entity.Name,
                IsActive = entity.IsActive,
                ExpirationDate = entity.ExpirationDate,
                TenantId = entity.TenantId,
                RoleClaimIdentifications = identifications
            };

            _memoryCache.Set(cacheKey, entry, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            });
        }

        if (!entry.IsActive)
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }

        if (entry.ExpirationDate.HasValue && entry.ExpirationDate.Value < DateTime.UtcNow)
        {
            return AuthenticateResult.Fail("API Key expired");
        }

        var principal = BuildPrincipal(entry);

        try
        {
            await _usageTracker.RecordSuccessfulAuthenticationAsync(entry.Id, cancellationToken);
            await _lastUsedThrottler.TouchIfDueAsync(entry.Id, cancellationToken);
        }
        catch
        {
            // best-effort; authentication still succeeds
        }

        return AuthenticateResult.Success(new AuthenticationTicket(principal, RbkAuthenticationSchemes.API_KEY));
    }

    private static ClaimsPrincipal BuildPrincipal(ApiKeyAuthCacheEntry entry)
    {
        var identity = new ClaimsIdentity(
            RbkAuthenticationSchemes.API_KEY,
            ClaimTypes.Name,
            JwtClaimIdentifiers.Roles);

        identity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Name, entry.Name));

        foreach (var role in entry.RoleClaimIdentifications)
        {
            identity.AddClaim(new System.Security.Claims.Claim(JwtClaimIdentifiers.Roles, role));
        }

        identity.AddClaim(new System.Security.Claims.Claim(JwtClaimIdentifiers.Tenant, entry.TenantId ?? string.Empty));
        identity.AddClaim(new System.Security.Claims.Claim(JwtClaimIdentifiers.DisplayName, entry.Name));
        identity.AddClaim(new System.Security.Claims.Claim(JwtClaimIdentifiers.Avatar, string.Empty));
        identity.AddClaim(new System.Security.Claims.Claim(JwtClaimIdentifiers.AuthenticationMode, AuthenticationMode.ApiKey.ToString()));
        identity.AddClaim(new System.Security.Claims.Claim(RbkApiKeyClaimTypes.ApiKeyId, entry.Id.ToString()));

        return new ClaimsPrincipal(identity);
    }
}
