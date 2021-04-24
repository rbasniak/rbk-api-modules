using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Demo.BusinessLogic;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.Models;
using System;
using rbkApiModules.Demo.Database;
using rbkApiModules.Authentication;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Demo.Models;
using System.Linq;
using rbkApiModules.Demo.DataTransfer;

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

        [HttpGet("date-test1")]
        public async Task<ActionResult<DummyDate[]>> DateTest1()
        {
            return HttpResponse<DummyDate[]>(await Mediator.Send(new GetAllDemoUsers.Command()));
        }

        [HttpGet("date-test2")]
        public async Task<ActionResult<SubDummyDate>> DateTest2()
        {
            return HttpResponse<SubDummyDate>(await Mediator.Send(new GetAllDemoUsers.Command()));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SimpleNamedEntity>> Single(Guid id)
        {
            return HttpResponse<SimpleNamedEntity>(await Mediator.Send(new GetUserDetails.Command { Id = id }));
        }

        [AllowAnonymous]
        [HttpGet("test")]
        public async Task<ActionResult> Single([FromServices] DatabaseContext context)
        {
            var user1 = context.Set<BaseUser>().ToList().First();
            var user2 = context.Set<BaseUser>().ToList().Last();

            var user3 = await context.Set<ClientUser>().Include(x => x.Client).FirstAsync();
            var user4 = await context.Set<ManagerUser>().Include(x => x.Manager).FirstAsync();

            var client = await context.Set<Client>().Include(x => x.User).FirstAsync();
            var manager = await context.Set<Manager>().Include(x => x.User).FirstAsync();

            return Ok("Ok");
        }
    }
}
