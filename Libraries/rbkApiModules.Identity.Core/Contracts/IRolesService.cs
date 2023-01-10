namespace rbkApiModules.Identity.Core;

public interface IRolesService
{
    Task<Role[]> GetAllAsync(CancellationToken cancellation = default);
    Task<Role[]> FindByNameAsync(string name, CancellationToken cancellation = default);
    Task<Role> FindAsync(Guid id, CancellationToken cancellation = default);
    Task<Role> CreateAsync(Role role, CancellationToken cancellation = default );
    Task DeleteAsync(Guid id, CancellationToken cancellation = default );
    Task<Role> GetDetailsAsync(Guid id, CancellationToken cancellation = default );
    Task RenameAsync(Guid id, string name, CancellationToken cancellation = default);
    Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellation = default);
    Task UpdateRoleClaims(Guid id, Guid[] claimsIds, CancellationToken cancellation = default);
    Task DeleteRolesFromTenant(string tenant, CancellationToken cancellation = default);
}
