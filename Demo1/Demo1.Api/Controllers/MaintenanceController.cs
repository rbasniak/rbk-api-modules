using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;

namespace Demo1.Api.Controllers;

[Authorize]
[IgnoreOnCodeGeneration(IgnoreMode.StateOnly)]
[ApiController]
[Route("api/[controller]")]
public class MaintenanceController: BaseController
{
    public MaintenanceController(Serilog.ILogger logger1, ILogger<MaintenanceController> logger2)
    {
        logger1.Fatal("Hello from Serilog");
        logger2.LogCritical("Hello from Microsoft");
    }

    [HttpGet]
    public string Test()
    {
        return "Ok";
    }
}
