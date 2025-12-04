using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Relational;

public class RelationalTenantsService : ITenantsService
{
    private readonly DbContext _context;
    private readonly ITenantPostCreationAction _postCreationAction;

    public RelationalTenantsService(IEnumerable<DbContext> contexts, ITenantPostCreationAction postCreationAction)
    {
        _context = contexts.GetDefaultContext();
        _postCreationAction = postCreationAction;
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken )
    {
        await _context.AddAsync(tenant, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _postCreationAction.ExecuteAsync(tenant, cancellationToken);

        return tenant;
    }

    public async Task DeleteAsync(string alias, CancellationToken cancellationToken )
    {
        var tenant = await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellationToken);

        _context.Remove(tenant);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Tenant> FindAsync(string alias, CancellationToken cancellationToken)
    {
        return await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellationToken);
    }

    public async Task<Tenant> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Set<Tenant>().SingleOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper(), cancellationToken);
    }

    public async Task<Tenant[]> GetAllAsync(CancellationToken cancellationToken)
    {
        var results = await _context.Set<Tenant>()
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellationToken);

        return results;
    }

    public async Task<Tenant> GetDetailsAsync(string alias, CancellationToken cancellationToken)
    {
        return await _context.Set<Tenant>().FindAsync(alias.ToUpper(), cancellationToken);
    }

    public async Task<Tenant> UpdateAsync(string alias, string name, string metadata, CancellationToken cancellationToken)
    {
        var tenant = await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellationToken);

        tenant.Update(name, metadata);

        await _context.SaveChangesAsync(cancellationToken);

        return tenant;
    } 
}
