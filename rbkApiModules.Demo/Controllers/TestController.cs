using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities.Extensions;
using System.IO;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController: BaseController
    {
        [HttpGet("download")]
        public ActionResult Get()
        {
            var memory = new MemoryStream();
            using (var stream = new FileStream(@"D:\Repositories\tecgraf\e-libra\back\Libra.Api\wwwroot\files\storage\libras\2cc36e08-fbab-4a44-d593-08d88b038b7a\attachments\627f4943-9e4b-4e2c-8d82-eacd969798f5.docx", FileMode.Open))
            {
                stream.CopyTo(memory);
            }
            memory.Position = 0;

            return File(memory, "application/octet-stream", "myfile.dat");
        }

        [ApplicationArea("Test")]
        [HttpGet]
        [Route("test1")]
        public ActionResult Test1()
        {
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
