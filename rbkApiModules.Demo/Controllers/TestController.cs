using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities.Extensions;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController: BaseController
    {
        [ApplicationArea("Test")]
        [HttpGet]
        [Route("test1")]
        public ActionResult Test1()
        {
            throw new System.DivideByZeroException("WTF?!");
            HttpContext.SetResponseSource();
            return StatusCode(500);
        }

        [HttpGet]
        [Route("test2/{id}")]
        [ApplicationArea("Test")]
        public ActionResult Test2(int id)
        {
            return Ok("Ok: " + id);
        }
    }
}
