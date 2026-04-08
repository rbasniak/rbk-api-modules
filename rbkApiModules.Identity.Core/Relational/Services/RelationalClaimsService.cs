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

    public async Task<Claim[]> GetAllAsync(CancellationToken cancellationToken)
    {
        var results = await _context.Set<Claim>()
            .OrderBy(x => x.Description)
            .Where(x => x.Hidden == false)
            .ToArrayAsync(cancellationToken);

        return results;
    }

    public async Task<Claim> FindAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<Claim> FindByIdentificationAsync(string identification, CancellationToken cancellationToken)
    {
        return await _context.Set<Claim>().FirstOrDefaultAsync(x => x.Identification.ToUpper() == identification.ToUpper());
    }

    public async Task<Claim> CreateAsync(Claim claim, CancellationToken cancellationToken)
    {
        await _context.AddAsync(claim, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return claim;
    }

    public async Task<bool> IsUsedByAnyRolesAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        return await _context.Set<Role>()
            .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
            .AnyAsync(role => role.Claims.Any(x => x.ClaimId == id), cancellationToken);
    }

    public async Task<bool> IsUsedByAnyUsersAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        return await _context.Set<User>()
            .Include(x => x.Claims).ThenInclude(x => x.Claim)
            .AnyAsync(user =>
                user.Claims.Any(x => x.ClaimId == id));
    }

    public async Task ProtectAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        claim.Protect();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UnprotectAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        claim.Unprotect();

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        _context.Remove(claim);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RenameAsync(Guid id, string description, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        claim.SetDescription(description);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetAllowApiKeyUsageAsync(Guid id, bool allowApiKeyUsage, CancellationToken cancellationToken)
    {
        var claim = await _context.Set<Claim>().FindAsync(new object[] { id }, cancellationToken);

        if (allowApiKeyUsage)
        {
            claim.AllowUsageOnApiKeys();
        }
        else
        {
            claim.RestrictUsageOnApiKeys();
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddClaimOverridesAsync(Guid[] claimIds, string username, string tenant, ClaimAccessType mode, CancellationToken cancellationToken)
    {
        var claims = await _context.Set<Claim>().Where(claim => claimIds.Any(id => claim.Id == id)).ToListAsync(cancellationToken);
        var user = await _authService.FindUserAsync(username, tenant, cancellationToken);

        await _context.Entry(user).Collection(x => x.Claims).LoadAsync(cancellationToken);

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

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveClaimOverridesAsync(Guid[] claimIds, string username, string tenant, CancellationToken cancellationToken)
    {
        var user = await _authService.FindUserAsync(username, tenant, cancellationToken);

        await _context.Entry(user).Collection(x => x.Claims).LoadAsync(cancellationToken);

        foreach (var claimId in claimIds)
        {
            var existingAssociation = user.Claims.FirstOrDefault(x => x.ClaimId == claimId);

            if (existingAssociation != null)
            {
                _context.Remove(existingAssociation);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Claim> FindByDescriptionAsync(string description, CancellationToken cancellationToken)
    {
        return await _context.Set<Claim>().FirstOrDefaultAsync(x => x.Description.ToLower() == description.ToLower(), cancellationToken);
    }
}
