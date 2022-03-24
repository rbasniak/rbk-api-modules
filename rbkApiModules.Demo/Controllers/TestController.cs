using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Authentication.AuthenticationGroups;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Demo.DataTransfer;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [CodeGenerationScope("project-c")]
    public class TestController: BaseController
    {
        [RbkAuthorize(Group = "client")]
        [HttpGet("test-all-clients")]
        public ActionResult TestAllClients()
        {
            var temp = HttpContext.User.Identity.IsAuthenticated;
            return Ok("Autorizado");
        }

        [RbkAuthorize(Claim = "SAMPLE_CLAIM", Group = "client")]
        [HttpGet("test-clients-with-claims")]
        public ActionResult TestClientsWithClaims()
        {
            return Ok("Autorizado");
        }

        [RbkAuthorize(Group = "manager")]
        [HttpGet("test-all-managers")]
        public ActionResult TestAllManagers()
        {
            return Ok("Autorizado");
        }

        [RbkAuthorize(Claim = "SAMPLE_CLAIM", Group = "manager")]
        [HttpGet("test-managers-with-claims")]
        public ActionResult TestManagersWithClaims()
        {
            return Ok("Autorizado");
        }

        [HttpGet("test-normal")]
        public ActionResult Test2()
        {
            return Ok("Autorizado");
        }

        [HttpPut]
        [AllowAnonymous]
        public async Task<ActionResult<DocumentDetails>> MessageTest(TestCommand.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse<DocumentDetails>(result);
        }
    }
}