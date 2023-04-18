using rbkApiModules.Identity.Core;
using Claim = System.Security.Claims.Claim;

namespace Demo3;

public class CustomClaimHandler : ICustomClaimHandler
{
    private readonly DatabaseContext _context;

    public CustomClaimHandler(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<Claim[]> GetClaims(string tenant, string username)
    {
        var user = _context.Users.FirstOrDefault(x => x.TenantId == tenant && (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username.ToLower()));

        if (user == null) 
        {
            throw new KeyNotFoundException("Could not find user");
        }

        var sector = String.Empty;
        var manager = String.Empty;

        if (user.Metadata != null)
        {
            user.Metadata.TryGetValue("sector", out sector);
            user.Metadata.TryGetValue("manager", out manager);
        }

        if (sector == null) sector = "unknown";
        if (manager == null) manager = "unknown";

        return await Task.FromResult(new[] 
        { 
            new Claim("sector", sector),
            new Claim("manager", manager),
        });
    }
}