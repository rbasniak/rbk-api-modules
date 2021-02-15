using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Demo.BusinessLogic;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : BaseController
    {
        [HttpPost]
        public async Task<ActionResult> Create(CreateUser.Command data)
        {
            return HttpResponse(await Mediator.Send(data));
        }

        [HttpGet]
        public async Task<ActionResult<SimpleNamedEntity[]>> All()
        {
            return HttpResponse<SimpleNamedEntity[]>(await Mediator.Send(new GetAllDemoUsers.Command()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleNamedEntity>> Single(Guid id)
        {
            return HttpResponse<SimpleNamedEntity>(await Mediator.Send(new GetUserDetails.Command { Id = id }));
        }
    }
}
