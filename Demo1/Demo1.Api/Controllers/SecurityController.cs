using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Demo1.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SecurityController : BaseController
{
    [HttpGet("ntlm")]
    public ActionResult NtlmTest()
    {
        return Ok(new 
        { 
            IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
            Username = HttpContext.User.Identity.Name,
        });
    }
}