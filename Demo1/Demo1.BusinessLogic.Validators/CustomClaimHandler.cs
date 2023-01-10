using Demo1.Database.Domain;
using rbkApiModules.Identity.Core;
using Claim = System.Security.Claims.Claim;

namespace Demo1.BusinessLogic.Validators;

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

        return await Task.FromResult(new[] 
        { 
            new Claim("last-login", user.LastLogin.HasValue ? user.LastLogin.Value.ToString() : String.Empty),
            new Claim("has-tenant", user.HasTenant.ToString()),
        });
    }
}