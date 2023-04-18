using Microsoft.EntityFrameworkCore;
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

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellation = default )
    {
        await _context.AddAsync(tenant, cancellation);

        await _context.SaveChangesAsync(cancellation);

        await _postCreationAction.ExecuteAsync(tenant, cancellation);

        return tenant;
    }

    public async Task DeleteAsync(string alias, CancellationToken cancellation = default )
    {
        var tenant = await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellation);

        _context.Remove(tenant);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task<Tenant> FindAsync(string alias, CancellationToken cancellation = default)
    {
        return await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellation);
    }

    public async Task<Tenant> FindByNameAsync(string name, CancellationToken cancellation = default)
    {
        return await _context.Set<Tenant>().SingleOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper(), cancellation);
    }

    public async Task<Tenant[]> GetAllAsync(CancellationToken cancellation = default)
    {
        var results = await _context.Set<Tenant>()
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellation);

        return results;
    }

    public async Task<Tenant> GetDetailsAsync(string alias, CancellationToken cancellation = default)
    {
        return await _context.Set<Tenant>().FindAsync(alias.ToUpper(), cancellation);
    }

    public async Task<Tenant> UpdateAsync(string alias, string name, string metadata, CancellationToken cancellation = default)
    {
        var tenant = await _context.Set<Tenant>().FindAsync(new object[] { alias.ToUpper() }, cancellation);

        tenant.Update(name, metadata);

        await _context.SaveChangesAsync(cancellation);

        return tenant;
    } 
}
