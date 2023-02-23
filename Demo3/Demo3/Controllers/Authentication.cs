using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;

namespace Demo3.Controllers;

[ApiController]
[Route("[controller]")]
public class Authentication : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login()
    {
        var username = HttpContext.User.Identity.Name;

        if (username == null) username = "admin";

        var issuer = "https://demo3.com/";
        var audience = "https://demo3.com/";
        var key = Encoding.ASCII.GetBytes("My super duper awesome secret key");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("id", Guid.NewGuid().ToString()),
                new Claim("username", username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
             }),
            Expires = DateTime.UtcNow.AddYears(5),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new { JwtToken = jwtToken });
    }

    [Authorize]
    [HttpGet("test")]
    public ActionResult Test()
    {
        var user = HttpContext.User.Identity.Name;

        if (HttpContext.User.Identity.IsAuthenticated)
        {
            return Ok(new { Authenticated = true });
        }
        else
        {
            return BadRequest();
        }
    }
}
