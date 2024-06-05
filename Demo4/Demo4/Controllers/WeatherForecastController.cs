using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Identity.Core;

namespace Demo4.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet("jwt-auth")]
        [Authorize]
        public string Jwt()
        {
            return "Succesfully authenticated with JWT token";
        }

        [HttpGet("key-auth")]
        [Authorize(AuthenticationSchemes = RbkAuthenticationSchemes.API_KEY)]
        public string ApiKey()
        {
            return "Succesfully authenticated with API key";
        }

        [HttpGet("anonymous")]
        [AllowAnonymous]
        public string Anonymous()
        {
            return "Anonymous user was able to access the data";
        }
    }
}