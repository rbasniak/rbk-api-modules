using Microsoft.IdentityModel.Tokens;

namespace rbkApiModules.Identity.Core;

public class JwtIssuerOptions
{
    public string Issuer { get; set; }
    public string Subject { get; set; }
    public string Audience { get; set; }
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssuedAt => DateTime.UtcNow;
    public SigningCredentials SigningCredentials { get; set; }
    public string SecretKey { get; set; }
    // Access token duration, in minutes
    public double AccessTokenLife { get; set; }
    // Refreshtoken duration, in minutes
    public double RefreshTokenLife { get; set; }
}