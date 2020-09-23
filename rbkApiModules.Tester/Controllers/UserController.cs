using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Tester.BusinessLogic;
using System.Threading.Tasks;

namespace rbkApiModules.Tester.Controllers
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
    }
}
