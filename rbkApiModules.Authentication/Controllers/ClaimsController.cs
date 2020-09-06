using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Authentication;
using rbkApiModules.Infrastructure;
using rbkApiModules.Infrastructure.Api.Controllers;
using rbkApiModules.Infrastructure.Models;
using System.Threading.Tasks;

namespace AspNetCoreApiTemplate.Api
{
    /// <summary>
    /// Controller para acessar as funcionalidades de contClaims de acesso
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClaimsController : BaseController
    {
        /// <summary>
        /// Lista as permissões de acesso
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<SimpleNamedEntity[]>> All()
        {
            return HttpResponse<SimpleNamedEntity[]>(await Mediator.Send(new GetAllClaims.Command()));
        }
    }
}
