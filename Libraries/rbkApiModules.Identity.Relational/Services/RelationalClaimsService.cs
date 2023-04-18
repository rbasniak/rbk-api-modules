using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Relational;

public class RelationalClaimsService : IClaimsService
{
    private readonly DbContext _context;
    private readonly IAuthService _authService;

    public RelationalClaimsService(IEnumerable<DbContext> contexts, IAuthService authService)
    {
        _context = contexts.GetDefaultContext();
        _authService = authService;
    }

    public async Task<Claim[]> GetAllAsync(CancellationToken cancellation = default)
    {
        var results = await _context.Set<Claim>()
            .OrderBy(x => x.Description)
            .Where(x => x.Hidden == false)
            .ToArrayAsync(cancellation);

        return results;
    }

    public async Task<Claim> FindAsync(Guid id, CancellationToken cancellation = default)
    {
        return await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);
    }

    public async Task<Claim> FindByIdentificationAsync(string identification, CancellationToken cancellation = default)
    {
        return await _context.Set<Claim>().FirstOrDefaultAsync(x => x.Identification.ToUpper() == identification.ToUpper());
    }

    public async Task<Claim> CreateAsync(Claim claim, CancellationToken cancellation = default)
    {
        await _context.AddAsync(claim, cancellation);

        await _context.SaveChangesAsync(cancellation);

        return claim;
    }

    public async Task<bool> IsUsedByAnyRolesAsync(Guid id, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        return await _context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .AnyAsync(role => role.Claims.Any(x => x.ClaimId == id), cancellation);
    }

    public async Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        return await _context.Set<User>()
            .Include(x => x.Claims).ThenInclude(x => x.Claim)
            .AnyAsync(user =>
                user.Claims.Any(x => x.ClaimId == id));
    }

    public async Task ProtectAsync(Guid id, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        claim.Protect();

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task UnprotectAsync(Guid id, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        claim.Unprotect();

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        _context.Remove(claim);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task RenameAsync(Guid id, string description, CancellationToken cancellation = default)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellation);

        claim.SetDescription(description);

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task AddClaimOverridesAsync(Guid[] claimIds, string username, string tenant, ClaimAccessType mode, CancellationToken cancellation = default)
    {
        var claims = await _context.Set<Claim>().Where(claim => claimIds.Any(id => claim.Id == id)).ToListAsync(cancellation);
        var user = await _authService.FindUserAsync(username, tenant, cancellation);

        await _context.Entry(user).Collection(x => x.Claims).LoadAsync(cancellation);

        foreach (var claim in claims)
        {
            var existingAssociation = user.Claims.FirstOrDefault(x => x.ClaimId == claim.Id);

            if (existingAssociation != null)
            {
                existingAssociation.SetAccessType(mode);
            }
            else
            {
                user.AddClaim(claim, mode);
            }
        }

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task RemoveClaimOverridesAsync(Guid[] claimIds, string username, string tenant, CancellationToken cancellation = default)
    {
        var user = await _authService.FindUserAsync(username, tenant, cancellation);

        await _context.Entry(user).Collection(x => x.Claims).LoadAsync(cancellation);

        foreach (var claimId in claimIds)
        {
            var existingAssociation = user.Claims.FirstOrDefault(x => x.ClaimId == claimId);

            if (existingAssociation != null)
            {
                _context.Remove(existingAssociation);
            }
        }

        await _context.SaveChangesAsync(cancellation);
    }

    public async Task<Claim> FindByDescriptionAsync(string description)
    {
        return await _context.Set<Claim>().FirstOrDefaultAsync(x => x.Description.ToLower() == description.ToLower());
    }
}
