using Microsoft.IdentityModel.Tokens;
using System;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Classe com os dados necessários para geração do token JWT
    /// </summary>
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }
        public string Subject { get; set; }
        public string Audience { get; set; } 
        public DateTime NotBefore => DateTime.UtcNow;
        public DateTime IssuedAt => DateTime.UtcNow;
        public SigningCredentials SigningCredentials { get; set; }
        public string SecretKey { get; set; }
        public int AccessTokenLife { get; set; }
        public int RefreshTokenLife { get; set; }
    }
}
