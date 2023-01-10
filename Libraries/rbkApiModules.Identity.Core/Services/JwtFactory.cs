using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace rbkApiModules.Identity.Core;

public interface IJwtFactory
{
    string GenerateEncodedToken(string username, Dictionary<string, string[]> roles);
}

/// <summary>
/// Classe para criação de tokens de acesso
/// </summary>
public class JwtFactory : IJwtFactory
{
    private readonly JwtIssuerOptions _jwtOptions;

    /// <summary>
    /// Construtor padrão
    /// </summary>
    public JwtFactory(IOptions<JwtIssuerOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
        ThrowIfInvalidOptions(_jwtOptions);
    }

    /// <summary>
    /// Método de geração do access token, com o usuário e permissões do usuário
    /// </summary>
    /// <param name="username">Nome do usuário</param>
    /// <param name="roles">Permissões do usuário (claims)</param>
    /// <returns>Access token</returns>
    public string GenerateEncodedToken(string username, Dictionary<string, string[]> roles)
    {
        var claims = new List<System.Security.Claims.Claim>
            {
                 new System.Security.Claims.Claim(JwtRegisteredClaimNames.Sub, username),
                 new System.Security.Claims.Claim(ClaimTypes.Name, username),
                 // new Claim(JwtRegisteredClaimNames.Iat, _jwtOptions.IssuedAt.ToUnixEpochDate().ToString(), ClaimValueTypes.Integer64),
            };

        foreach (var pair in roles)
        {
            foreach (var value in pair.Value)
            {
                claims.Add(new System.Security.Claims.Claim(pair.Key, value ?? ""));
            }
        }

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: _jwtOptions.NotBefore,
            expires: _jwtOptions.IssuedAt.Add(TimeSpan.FromMinutes(_jwtOptions.AccessTokenLife)),
            signingCredentials: _jwtOptions.SigningCredentials);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }

    /// <summary>
    /// Classe para disparar exceção caso algum dado esteja inválido
    /// </summary>
    /// <param name="options">Dados necessários para criação do token</param>
    private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));

        if (options.AccessTokenLife <= 0)
        {
            throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.AccessTokenLife));
        }

        if (options.SigningCredentials == null)
        {
            throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
        }

    }
}