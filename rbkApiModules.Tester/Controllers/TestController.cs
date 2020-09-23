using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Infrastructure.Api;

namespace rbkApiModules.Tester.Controllers
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
