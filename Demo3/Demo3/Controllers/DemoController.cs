using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Identity.Core;

namespace Demo3;

[ApiController]
[IgnoreOnCodeGeneration]
[Route("api/[controller]")]
public class DemoController : BaseController
{
    [AllowAnonymous]
    [HttpGet("anonymous")]
    public ActionResult<Response> GetAnonymous()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an open endpoint",
        });
    }

    [Authorize]
    [HttpGet("authorized/low-privilegies")]
    public ActionResult<Response> GetAuthorizedWithLowPrivilegies()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an authenticated endpoint with low privilegies",
        });
    }

    [RbkAuthorize(AuthenticationClaims.MANAGE_USERS)]
    [HttpGet("authorized/high-privilegies1")]
    public ActionResult<Response> GetAuthorizedWithHighPrivilegies1()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "Successfully accessed an authenticated endpoint with high privilegies",
        });
    }    
    
    [RbkAuthorize("NON_EXISTENT_CLAIM")]
    [HttpGet("authorized/high-privilegies2")]
    public ActionResult<Response> GetAuthorizedWithHighPrivilegies2()
    {
        return Ok(new Response
        {
            Timestamp = DateTime.Now,
            Message = "You should not have access to this",
        });
    }
}

public class Response
{
    public required DateTime Timestamp { get; set; }
    public required string Message { get; set; }
}
