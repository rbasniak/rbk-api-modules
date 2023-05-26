using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core;
using rbkApiModules.Comments.Core;

namespace rbkApiModules.Diagnostics.Core;

[Authorize]
[ApiController]
[IgnoreOnCodeGeneration]
[Route("api/[controller]")]
public class DiagnosticsController : BaseController
{
    [HttpGet]
    [Route("options")]
    public async Task<ActionResult<GetFilteringLists.Response>> GetFilterData()
    {
        var result = await Mediator.Send(new GetFilteringLists.Request());

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("list")]
    public async Task<ActionResult<LoadListData.Response>> Search([FromBody] LoadListData.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("dashboard")]
    public async Task<ActionResult<GetDashboardData.Response>> GetDashboardData([FromBody] GetDashboardData.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }
}
