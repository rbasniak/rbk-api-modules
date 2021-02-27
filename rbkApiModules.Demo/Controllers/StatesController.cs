using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Demo.BusinessLogic.StateMachine;
using rbkApiModules.Demo.DataTransfer;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities.Extensions;
using rbkApiModules.Workflow;
using System.IO;

namespace rbkApiModules.Demo.Controllers
{
    [IgnoreOnCodeGeneration]
    [Route("api/[controller]")]
    [ApiController]
    public class StatesController: BaseWorkflowController<
        GetStateDotCode.Command, 
        GetStateData.Command,
        GetQueryDefinitionResults.Command,
        GetStateChangedEvents.Command,
        DocumentDetails,
        StateChangedEventDetails>
    {
    }
}
