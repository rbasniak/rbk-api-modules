namespace rbkApiModules.Identity.Core;

public interface ICustomClaimHandler
{
    Task<System.Security.Claims.Claim[]> GetClaims(string tenant, string username);
}
