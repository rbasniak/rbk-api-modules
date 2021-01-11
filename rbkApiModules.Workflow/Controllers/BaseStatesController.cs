using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseStatesController<TBaseGetStatesDotCodeCommand, TBaseGetStateDataCommand> : BaseController
        where TBaseGetStatesDotCodeCommand: BaseGetStatesDotCodeCommand, new()
        where TBaseGetStateDataCommand : BaseGetStateDataCommand, new()
    {
        /// <summary>
        /// Generates the dot code of the states/transitions of the specified group
        /// </summary>
        [HttpGet("dot-code/{id}")]
        public async Task<ActionResult<DotCodeResponse>> GenerateDotCode(Guid id)
        {
            var response = await Mediator.Send(new TBaseGetStatesDotCodeCommand { GroupId = id });

            return HttpResponse(response);
        }

        /// <summary>
        /// Returns a hierarchycal model of all groups of state/transitions 
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<StateGroupDetails[]>> All()
        {
            var response = await Mediator.Send(new TBaseGetStateDataCommand());

            return HttpResponse(response);
        }
    }
}
