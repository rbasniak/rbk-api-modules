using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Relational;

public class RelationalRolesService : IRolesService
{
    private readonly DbContext _context;

    public RelationalRolesService(IEnumerable<DbContext> contexts)
    {
        _context = contexts.GetDefaultContext();
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken cancellationToken)
    {
        await _context.AddAsync(role, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return role;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var role2 = await _context.Set<Role>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        var role = await _context.Set<Role>().IgnoreQueryFilters().Include(x => x.Claims).FirstAsync(x => x.Id == id);

        _context.RemoveRange(role.Claims);
        _context.Remove(role);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRolesFromTenant(string tenant, CancellationToken cancellationToken)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var roles = await _context.Set<Role>()
            .IgnoreQueryFilters()
            .Include(x => x.Users)
            .Include(x => x.Claims)
            .Where(x => x.TenantId == tenant).ToListAsync();

        _context.RemoveRange(roles);

        await _context.SaveChangesAsync();
    }

    public async Task<Role> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Role>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<Role[]> FindByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Set<Role>().IgnoreQueryFilters().Where(x => x.Name.ToUpper() == name.ToUpper()).ToArrayAsync(cancellationToken);
    }

    public async Task<Role[]> GetAllAsync(CancellationToken cancellationToken)
    {
        var results = await _context.Set<Role>()
                .IgnoreQueryFilters()
                .Include(x => x.Claims).ThenInclude(x => x.Claim)
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellationToken);

        return results;
    }

    public async Task<Role> GetDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Role>()
            .IgnoreQueryFilters()
            .Include(x => x.Claims)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<UserToRole>().AnyAsync(x => x.RoleId == id, cancellationToken);
    }

    public async Task RenameAsync(Guid id, string name, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Role>().IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        claim.Rename(name);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRoleClaims(Guid roleId, Guid[] claimsIds, CancellationToken cancellationToken)
    {
        var role = await _context.Set<Role>()
           .IgnoreQueryFilters()
           .Include(x => x.Claims)
               .SingleAsync(x => x.Id == roleId, cancellationToken);

        _context.RemoveRange(role.Claims);

        foreach (var claimId in claimsIds)
        {
            var claim = await _context.Set<Claim>().SingleAsync(x => x.Id == claimId, cancellationToken);
            role.AddClaim(claim);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
