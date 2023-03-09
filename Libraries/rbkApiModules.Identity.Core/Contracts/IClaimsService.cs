namespace rbkApiModules.Identity.Core;

public interface IClaimsService
{
    Task<Claim[]> GetAllAsync(CancellationToken cancellation = default);
    Task<Claim> FindAsync(Guid id, CancellationToken cancellation = default);
    Task<Claim> FindByIdentificationAsync(string identification, CancellationToken cancellation = default);
    Task<Claim> CreateAsync(Claim claim, CancellationToken cancellation = default);
    Task<bool> IsUsedByAnyRolesAsync(Guid id, CancellationToken cancellation = default);
    Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellation = default);
    Task ProtectAsync(Guid id, CancellationToken cancellation = default);
    Task UnprotectAsync(Guid id, CancellationToken cancellation = default);
    Task DeleteAsync(Guid id, CancellationToken cancellation = default);
    Task RenameAsync(Guid id, string description, CancellationToken cancellation = default);
    Task AddClaimOverridesAsync(Guid[] claimIds, string username, string tenant, ClaimAccessType mode, CancellationToken cancellation = default);
    Task RemoveClaimOverridesAsync(Guid[] claimIds, string username, string tenant, CancellationToken cancellation = default);
    Task<Claim> FindByDescriptionAsync(string description);
}
