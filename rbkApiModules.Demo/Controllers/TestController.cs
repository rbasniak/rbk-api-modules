using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Authentication.AuthenticationGroups;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities;
using rbkApiModules.Utilities.Extensions;
using System;
using System.IO;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController: BaseController
    {
        [RbkAuthorize(Group = "client")]
        [HttpGet("test-all-clients")]
        public ActionResult TestAllClients()
        {
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
    }
}