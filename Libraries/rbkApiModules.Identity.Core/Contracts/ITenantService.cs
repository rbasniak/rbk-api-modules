namespace rbkApiModules.Identity.Core;

public interface ITenantsService
{
    Task<Tenant[]> GetAllAsync(CancellationToken cancellation = default);
    Task<Tenant> FindAsync(string alias, CancellationToken cancellation = default);
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellation = default);
    Task DeleteAsync(string alias, CancellationToken cancellation = default);
    Task<Tenant> GetDetailsAsync(string alias, CancellationToken cancellation = default);
    Task<Tenant> UpdateAsync(string alias, string name, string metadata, CancellationToken cancellation = default);
    Task<Tenant> FindByNameAsync(string name, CancellationToken cancellation = default);
}
