using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Identity.Core;

public sealed class ApiKey : BaseEntity
{
    public const int MinRequestsPerMinute = 1;

    public const int MaxRequestsPerMinute = 100_000;

    private HashSet<ApiKeyToClaim> _claims;

    private ApiKey()
    {
        _claims = default!;
    }

    public ApiKey(string name, string keyHash, string keyPrefix, string? tenantId, DateTime? expirationDate, int requestsPerMinute, int burstLimit)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (string.IsNullOrWhiteSpace(keyHash))
        {
            throw new ArgumentNullException(nameof(keyHash));
        }

        if (string.IsNullOrWhiteSpace(keyPrefix))
        {
            throw new ArgumentNullException(nameof(keyPrefix));
        }

        ValidateRateLimits(requestsPerMinute, burstLimit);

        Name = name;
        KeyHash = keyHash;
        KeyPrefix = keyPrefix;
        TenantId = NormalizeTenantId(tenantId);
        ExpirationDate = expirationDate;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        RequestsPerMinute = requestsPerMinute;
        BurstLimit = burstLimit;
        _claims = new HashSet<ApiKeyToClaim>();
    }

    [Required, MinLength(1), MaxLength(256)]
    public string Name { get; private set; } = string.Empty;

    [Required, MaxLength(512)]
    public string KeyHash { get; private set; } = string.Empty;

    [Required, MaxLength(64)]
    public string KeyPrefix { get; private set; } = string.Empty;

    public DateTime? ExpirationDate { get; private set; }

    public bool IsActive { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? LastUsedAt { get; private set; }

    [MaxLength(255)]
    public string? TenantId { get; private set; }

    public DateTime? RevokeDate { get; private set; }

    [MaxLength(2048)]
    public string? RevokeReason { get; private set; }

    public int RequestsPerMinute { get; private set; }

    public int BurstLimit { get; private set; }

    public IEnumerable<ApiKeyToClaim> Claims => _claims?.AsReadOnly()!;

    public ApiKeyToClaim AddClaim(Claim claim)
    {
        if (_claims == null)
        {
            throw new ApplicationException("ApiKey relationships need to be fully loaded from database.");
        }

        var link = new ApiKeyToClaim(this, claim);
        _claims.Add(link);
        return link;
    }

    public void ReplaceClaims(IReadOnlyCollection<Claim> claims)
    {
        if (_claims == null)
        {
            throw new ApplicationException("ApiKey relationships need to be fully loaded from database.");
        }

        _claims.Clear();

        foreach (var claim in claims)
        {
            _claims.Add(new ApiKeyToClaim(this, claim));
        }
    }

    public Claim[] GetAccessClaims()
    {
        if (_claims == null)
        {
            throw new ApplicationException("ApiKey relationships need to be fully loaded from database to check the access claims");
        }

        var result = new List<Claim>();

        foreach (var link in _claims)
        {
            if (link.Claim == null)
            {
                throw new ApplicationException("ApiKey relationships need to be fully loaded from database to check the access claims");
            }

            result.Add(link.Claim);
        }

        return result.OrderBy(x => x.Description).ToArray();
    }

    public void MarkUsed()
    {
        LastUsedAt = DateTime.UtcNow;
    }

    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
    }

    public void SetExpirationDate(DateTime? expirationDate)
    {
        ExpirationDate = expirationDate;
    }

    public void SetIsActive(bool isActive)
    {
        IsActive = isActive;
    }

    public void SetRateLimits(int requestsPerMinute, int burstLimit)
    {
        ValidateRateLimits(requestsPerMinute, burstLimit);
        RequestsPerMinute = requestsPerMinute;
        BurstLimit = burstLimit;
    }

    public void Revoke(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentNullException(nameof(reason));
        }

        if (!IsActive)
        {
            throw new InvalidOperationException("The API key is already revoked.");
        }

        IsActive = false;
        RevokeDate = DateTime.UtcNow;
        RevokeReason = reason.Trim();
    }

    private static string? NormalizeTenantId(string? tenantId)
    {
        if (string.IsNullOrEmpty(tenantId))
        {
            return null;
        }

        return tenantId.ToUpperInvariant();
    }

    private static void ValidateRateLimits(int requestsPerMinute, int burstLimit)
    {
        if (requestsPerMinute < MinRequestsPerMinute || requestsPerMinute > MaxRequestsPerMinute)
        {
            throw new ExpectedInternalException($"Requests per minute must be between {MinRequestsPerMinute} and {MaxRequestsPerMinute}.");
        }

        if (burstLimit < requestsPerMinute)
        {
            throw new ExpectedInternalException("Burst limit must be greater than or equal to requests per minute.");
        }
    }
}
