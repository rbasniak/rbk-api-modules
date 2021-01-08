using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController : BaseController
    {
        /// <summary>
        /// Retorna o cdigo DOT do diagrama de estados
        /// </summary>
        [HttpGet("dot-code")]
        public async Task<ActionResult<string>> DotCode()
        {
            var response = await Mediator.Send(new GetStatesDotCode.Command());

            return HttpResponse(response);
        }

        /// <summary>
        /// Retorna a lista de status e transições
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<StateGroupDetails[]>> All()
        {
            var response = await Mediator.Send(new GetAllStates.Command());

            return HttpResponse<StateGroupDetails[]>(response);
        }
    }
}
