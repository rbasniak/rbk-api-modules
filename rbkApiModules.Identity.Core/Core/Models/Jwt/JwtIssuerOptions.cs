using Microsoft.IdentityModel.Tokens;

namespace rbkApiModules.Identity.Core;

public class JwtIssuerOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public DateTime NotBefore => DateTime.UtcNow;
    public DateTime IssuedAt => DateTime.UtcNow;
    public SigningCredentials? SigningCredentials { get; set; } = null;
    public string SecretKey { get; set; } = string.Empty;   
    // Access token duration, in minutes
    public double AccessTokenLife { get; set; } = 0;
    // Refreshtoken duration, in minutes
    public double RefreshTokenLife { get; set; } = 0;
}