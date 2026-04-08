namespace rbkApiModules.Identity.Core;

public interface IClaimsService
{
    Task<Claim[]> GetAllAsync(CancellationToken cancellationToken);
    Task<Claim> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<Claim> FindByIdentificationAsync(string identification, CancellationToken cancellationToken);
    Task<Claim> CreateAsync(Claim claim, CancellationToken cancellationToken);
    Task<bool> IsUsedByAnyRolesAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellationToken);
    Task ProtectAsync(Guid id, CancellationToken cancellationToken);
    Task UnprotectAsync(Guid id, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task RenameAsync(Guid id, string description, CancellationToken cancellationToken);
    Task SetAllowApiKeyUsageAsync(Guid id, bool allowApiKeyUsage, CancellationToken cancellationToken);
    Task AddClaimOverridesAsync(Guid[] claimIds, string username, string tenant, ClaimAccessType mode, CancellationToken cancellationToken);
    Task RemoveClaimOverridesAsync(Guid[] claimIds, string username, string tenant, CancellationToken cancellationToken);
    Task<Claim> FindByDescriptionAsync(string description, CancellationToken cancellationToken);
}
