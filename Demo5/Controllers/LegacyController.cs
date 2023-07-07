
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Commons.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Demo5;

[AllowAnonymous]
[ApiController]
public class LegacyController : BaseController
{
    public const string AUTH = "api/legacy/token";

    private readonly SigningCredentials _signingCredentials;
    public LegacyController()
    {
        var privateKeyText = System.IO.File.ReadAllText("test.key.pem");
        var rsa = RSA.Create();
        rsa.ImportFromPem(privateKeyText);

        _signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256);
    }

    [HttpPost(AUTH)]
    [AllowAnonymous]
    public ActionResult GetServiceToken(bool withClaim = false)
    {
        var jwt = new JwtSecurityToken(
            audience: "LEGACY APPLICATION",
            issuer: "LEGACY AUTHENTICATION",
            claims: withClaim ? new Claim[] { new Claim(JwtClaimIdentifiers.Roles, "RESOURCE::READ") } : Array.Empty<Claim>(),
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddSeconds(600),
            signingCredentials: _signingCredentials
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return Ok(new { AccessToken = token });
    }
}