using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using System.Security.Cryptography;

namespace rbkApiModules.Identity.Relational;

public static class ApiKeySeeding
{
    public sealed record ApiKeySeedResult(Guid ApiKeyId, string RawKey);

    public static async Task<ApiKeySeedResult> SeedApiKeyAsync(
        DbContext context,
        string name,
        IReadOnlyList<Guid> claimIds,
        string? key,
        DateTime? expirationDate,
        string? tenantId,
        string prefix = "",
        CancellationToken cancellationToken = default)
    {
        if (claimIds == null || claimIds.Count == 0)
        {
            throw new ArgumentException("At least one claim is required.", nameof(claimIds));
        }

        var distinctIds = claimIds.Distinct().ToArray();

        if (distinctIds.Length != claimIds.Count)
        {
            throw new ArgumentException("Duplicate claim ids.", nameof(claimIds));
        }

        var claims = await context.Set<Claim>().Where(x => distinctIds.Contains(x.Id)).ToArrayAsync(cancellationToken);

        if (claims.Length != distinctIds.Length)
        {
            throw new InvalidOperationException("One or more claims were not found.");
        }

        foreach (var claim in claims)
        {
            if (!claim.AllowApiKeyUsage)
            {
                throw new InvalidOperationException($"Claim '{claim.Identification}' is not allowed on API keys.");
            }
        }

        string raw;
        string storedPrefix;
        string hash;

        if (string.IsNullOrEmpty(key))
        {
            var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant();
            (raw, storedPrefix, hash) = ApiKeyMaterial.Compose(secret, prefix);
        }
        else
        {
            (raw, storedPrefix, hash) = ApiKeyMaterial.Compose(key, prefix);
        }

        const int defaultRequestsPerMinute = 600;
        var apiKey = new ApiKey(name, hash, storedPrefix, tenantId, expirationDate, defaultRequestsPerMinute, defaultRequestsPerMinute);

        foreach (var claim in claims)
        {
            apiKey.AddClaim(claim);
        }

        await context.AddAsync(apiKey, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return new ApiKeySeedResult(apiKey.Id, raw);
    }
}
