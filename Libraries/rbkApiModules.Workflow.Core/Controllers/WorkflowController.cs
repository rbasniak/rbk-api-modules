using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public abstract class WorkflowController : BaseController
{
    /// <summary>
    /// Generates the dot code of the states/transitions of the specified group
    /// </summary>
    [HttpGet("general/dot-code/{groupId}")]
    public async Task<ActionResult<GetStatesDotCode.Response>> GenerateDotCode(Guid groupId)
    {
        var response = await Mediator.Send(new GetStatesDotCode.Request { GroupId = groupId });

        return HttpResponse(response);
    }

    /// <summary>
    /// Returns a hierarchycal model of all groups of state/transitions 
    /// </summary>
    [HttpGet("general/all")]
    public async Task<ActionResult<StateGroupDetails[]>> All()
    {
        var response = await Mediator.Send(new GetStateData.Request());

        return HttpResponse<StateGroupDetails[]>(response);
    }

    /// <summary>
    /// Executes a group of query definitions and return their results
    /// </summary>
    [HttpPost("queries")]
    public async Task<ActionResult<QueryDefinitionResults<StateEntityDetails>[]>> ExecuteQueries(GetQueryDefinitionResults.Request data)
    {
        var response = await Mediator.Send(data);

        return HttpResponse<QueryDefinitionResults<StateEntityDetails>[]> (response);
    }

    /// <summary>
    /// Executes
    /// </summary>
    [HttpGet("events/history/{entityId}")]
    public async Task<ActionResult<StateChangedEventDetails[]>> GetEntityHistory(Guid entityId)
    {
        var response = await Mediator.Send(new GetStateChangedEvents.Request { Id = entityId });

        return HttpResponse<StateChangedEventDetails[]>(response);
    }
}
