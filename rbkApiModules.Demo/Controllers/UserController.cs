using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Demo.BusinessLogic;
using System.Threading.Tasks;

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
    }
}
