using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Relational;

public class RelationalRolesService: IRolesService
{
    private readonly DbContext _context;

    public RelationalRolesService(IEnumerable<DbContext> contexts)
    {
        _context = contexts.GetDefaultContext();
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken cancellation = default )
    {
        await _context.AddAsync(role, cancellation);

        await _context.SaveChangesAsync(cancellation);

        return role;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellation = default )
    {
        var role = await _context.Set<Role>().FindAsync(id, cancellation);

        _context.Remove(role);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task DeleteRolesFromTenant(string tenant, CancellationToken cancellation = default)
    {
        tenant = tenant != null ? tenant.ToUpper() : null;

        var roles = await _context.Set<Role>()
            .Include(x => x.Users)
            .Include(x => x.Claims)
            .Where(x => x.TenantId == tenant).ToListAsync();

        _context.RemoveRange(roles);
        
        await _context.SaveChangesAsync();
    }

    public async Task<Role> FindAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<Role>().FindAsync(id, cancellation);
    }

    public async Task<Role[]> FindByNameAsync(string name, CancellationToken cancellation = default)
    {
        return await _context.Set<Role>().Where(x => x.Name.ToUpper() == name.ToUpper()).ToArrayAsync(cancellation);
    }

    public async Task<Role[]> GetAllAsync(CancellationToken cancellation = default)
    {
        var results = await _context.Set<Role>()
                .Include(x => x.Claims).ThenInclude(x => x.Claim)
                .OrderBy(x => x.Name)
                .ToArrayAsync(cancellation);

        return results;
    }

    public async Task<Role> GetDetailsAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<Role>()
            .Include(x => x.Claims)
            .FirstOrDefaultAsync(x => x.Id == id, cancellation);
    }

    public async Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<UserToRole>().AnyAsync(x => x.RoleId == id, cancellation);
    }

    public async Task RenameAsync(Guid id, string name, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Role>().FindAsync(id, cancellation);

        claim.Rename(name);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task UpdateRoleClaims(Guid roleId, Guid[] claimsIds, CancellationToken cancellation = default)
    {
        var role = await _context.Set<Role>()
           .Include(x => x.Claims)
               .SingleAsync(x => x.Id == roleId, cancellation);

        _context.RemoveRange(role.Claims);

        foreach (var claimId in claimsIds)
        {
            var claim = await _context.Set<Claim>().SingleAsync(c => c.Id == claimId, cancellation);
            role.AddClaim(claim);
        }

        await _context.SaveChangesAsync(cancellation);
    }
}
