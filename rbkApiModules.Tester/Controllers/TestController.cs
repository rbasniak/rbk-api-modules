using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;

namespace rbkApiModules.Tester.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController: BaseController
    {
        [HttpPost]
        [Route("test1")]
        public ActionResult Test1()
        {
            throw new Exception("xxxxxxxxxxxxxxxxxxx");
            return Ok("Ok");
        }

        [HttpGet]
        [Route("test2/{id}")]
        public ActionResult Test2(int id)
        {
            return Ok("Ok: " + id);
        }
    }
}
