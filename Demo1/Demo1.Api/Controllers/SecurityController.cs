using Microsoft.AspNetCore.Mvc;
using Demo1.BusinessLogic.Commands;
using Demo1.DataTransfer;
using rbkApiModules.Commons.Core;
using Demo1.BusinessLogic.Queries;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Demo1.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SecurityController : BaseController
{
    [HttpGet("ntlm")]
    public async Task<ActionResult> NtlmTest()
    {
        return Ok(new 
        { 
            IsAuthenticated = HttpContext.User.Identity.IsAuthenticated,
            Username = HttpContext.User.Identity.Name,
        });
    }
}