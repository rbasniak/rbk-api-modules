namespace rbkApiModules.Identity.Core;

public interface ITenantsService
{
    Task<Tenant[]> GetAllAsync(CancellationToken cancellationToken);
    Task<Tenant> FindAsync(string alias, CancellationToken cancellationToken);
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken);
    Task DeleteAsync(string alias, CancellationToken cancellationToken);
    Task<Tenant> GetDetailsAsync(string alias, CancellationToken cancellationToken);
    Task<Tenant> UpdateAsync(string alias, string name, string metadata, CancellationToken cancellationToken);
    Task<Tenant> FindByNameAsync(string name, CancellationToken cancellationToken);
}
