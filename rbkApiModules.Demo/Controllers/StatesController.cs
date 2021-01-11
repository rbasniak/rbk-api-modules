using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Demo.BusinessLogic.StateMachine;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Utilities.Extensions;
using rbkApiModules.Workflow;
using System.IO;

namespace rbkApiModules.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class StatesController: BaseStatesController<
        GetStateDotCode.Command, 
        GetStateData.Command
    >
    {
    }
}
