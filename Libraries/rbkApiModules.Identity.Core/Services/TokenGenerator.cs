using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class TokenGenerator
{
    public static JwtResponse Generate(IJwtFactory jwtFactory, User user, Dictionary<string, string[]> extraClaims)
    {
        var claims = new Dictionary<string, string[]>
        {
            { JwtClaimIdentifiers.Roles, user.GetAccessClaims().Select(x => x.Identification).ToArray() },
            { JwtClaimIdentifiers.Avatar, new string[] { user.Avatar } },
            { JwtClaimIdentifiers.Tenant, new string[] { user.TenantId } },
            { JwtClaimIdentifiers.DisplayName, new string[] { string.IsNullOrEmpty(user.DisplayName) ? user.Username : user.DisplayName } }
        };

        if (extraClaims != null)
        {
            foreach (var claim in extraClaims)
            {
                claims.Add(claim.Key, claim.Value);
            }
        }

        var response = new JwtResponse
        {
            AccessToken = jwtFactory.GenerateEncodedToken(user.Username, claims),
            RefreshToken = user.RefreshToken,
        };

        return response;
    }
}