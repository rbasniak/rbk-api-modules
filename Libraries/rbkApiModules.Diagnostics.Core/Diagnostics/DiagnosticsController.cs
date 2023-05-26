using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Diagnostics.Core;

[Authorize]
[ApiController]
[IgnoreOnCodeGeneration]
[Route("api/[controller]")]
public class DiagnosticsController : BaseController
{
    //[HttpGet]
    //[Route("filter-options")]
    //public async Task<ActionResult<FilterDiagnosticsEntries>> GetFilterData()
    //{
    //    var result = await Mediator.Send(new GetFilteringLists.Command());

    //    return HttpResponse(result);
    //}

    //[HttpPost]
    //[Route("search")]
    //public async Task<ActionResult<FilterDiagnosticsEntries.Results>> Search([FromBody] FilterDiagnosticsEntries.Command data)
    //{
    //    var result = await Mediator.Send(data);

    //    return HttpResponse(result);
    //}

    //[HttpPost]
    //[Route("dashboard")]
    //public async Task<ActionResult<DiagnosticsDashboard>> GetDashboardData([FromBody] GetDashboardData.Command data)
    //{
    //    var result = await Mediator.Send(data);

    //    return HttpResponse(result);
    //} 
}
