using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.CodeGeneration;

namespace Demo1.Api.Controllers;

[IgnoreOnCodeGeneration(IgnoreMode.StateOnly)]
[ApiController]
[Route("api/[controller]")]
public class MaintenanceController: BaseController
{
    [HttpGet]
    public string Test()
    {
        return "Ok";
    }
}
