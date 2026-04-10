namespace rbkApiModules.Identity.Core;

public interface IRolesService
{
    Task<Role[]> GetAllAsync(CancellationToken cancellationToken);
    Task<Role[]> FindByNameAsync(string name, CancellationToken cancellationToken);
    Task<Role> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken );
    Task DeleteAsync(Guid id, CancellationToken cancellationToken );
    Task<Role> GetDetailsAsync(Guid id, CancellationToken cancellationToken );
    Task RenameAsync(Guid id, string name, CancellationToken cancellationToken);
    Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateRoleClaims(Guid id, Guid[] claimsIds, CancellationToken cancellationToken);
    Task DeleteRolesFromTenant(string tenant, CancellationToken cancellationToken);
}
