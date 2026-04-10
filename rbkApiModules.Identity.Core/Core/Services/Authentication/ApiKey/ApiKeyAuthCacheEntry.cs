namespace rbkApiModules.Identity.Core;

internal sealed class ApiKeyAuthCacheEntry
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }

    public required bool IsActive { get; init; }

    public DateTime? ExpirationDate { get; init; }

    public string? TenantId { get; init; }

    public required IReadOnlyList<string> RoleClaimIdentifications { get; init; }
}
