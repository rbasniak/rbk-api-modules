using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.MediatR.Core;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public abstract class BaseWorkflowController<TBaseGetStatesDotCodeCommand, TBaseGetStateDataCommand, TBaseGetQueryDefinitionResultsCommand,
        TBaseGetStateChangedEventsCommand, TStateEntityDto, TStateChangeEventDto> : BaseController
        where TBaseGetStatesDotCodeCommand: BaseGetStatesDotCodeCommand, new()
        where TBaseGetStateDataCommand : BaseGetStateDataCommand, new()
        where TBaseGetStateChangedEventsCommand : BaseGetStateChangedEventsCommand, new()
    {
        /// <summary>
        /// Generates the dot code of the states/transitions of the specified group
        /// </summary>
        [HttpGet("general/dot-code/{groupId}")]
        public async Task<ActionResult<DotCodeResponse>> GenerateDotCode(Guid groupId)
        {
            var response = await Mediator.Send(new TBaseGetStatesDotCodeCommand { GroupId = groupId });

            return HttpResponse(response);
        }

        /// <summary>
        /// Returns a hierarchycal model of all groups of state/transitions 
        /// </summary>
        [HttpGet("general/all")]
        public async Task<ActionResult<StateGroupDetails[]>> All()
        {
            var response = await Mediator.Send(new TBaseGetStateDataCommand());

            return HttpResponse(response);
        }

        /// <summary>
        /// Executes a group of query definitions and return their results
        /// </summary>
        [HttpPost("queries")]
        public async Task<ActionResult<QueryDefinitionResults<TStateEntityDto>[]>> ExecuteQueries(TBaseGetQueryDefinitionResultsCommand data)
        {
            var response = (QueryResponse)await Mediator.Send(data);

            return HttpResponse(response);
        }

        /// <summary>
        /// Executes
        /// </summary>
        [HttpGet("events/history/{libraId}")]
        public async Task<ActionResult<TStateChangeEventDto[]>> GetEntityHistory(Guid libraId)
        {
            var response = await Mediator.Send(new TBaseGetStateChangedEventsCommand { Id = libraId });

            return HttpResponse(response);
        }
    }
}
