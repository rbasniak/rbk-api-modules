namespace rbkApiModules.Identity.Core;

public sealed class ApiKeyDetails
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required string KeyPrefix { get; init; }

    public DateTime? ExpirationDate { get; init; }

    public bool IsActive { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime? LastUsedAt { get; init; }

    public string? TenantId { get; init; }

    public DateTime? RevokeDate { get; init; }

    public string? RevokeReason { get; init; }

    public required IReadOnlyList<ClaimDetails> AssignedClaims { get; init; }

    public static ApiKeyDetails FromModel(ApiKey entity)
    {
        var claims = entity.GetAccessClaims().Select(ClaimDetails.FromModel).ToList();

        return new ApiKeyDetails
        {
            Id = entity.Id,
            Name = entity.Name,
            KeyPrefix = entity.KeyPrefix,
            ExpirationDate = entity.ExpirationDate,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            LastUsedAt = entity.LastUsedAt,
            TenantId = entity.TenantId,
            RevokeDate = entity.RevokeDate,
            RevokeReason = entity.RevokeReason,
            AssignedClaims = claims
        };
    }
}


