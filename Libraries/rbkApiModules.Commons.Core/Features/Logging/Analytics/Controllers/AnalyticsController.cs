using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Commons.Analytics;
using rbkApiModules.Commons.Core.CodeGeneration;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

[ExcludeFromCodeCoverage]
[IgnoreOnCodeGeneration]
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : BaseController
{
    [HttpGet]
    [Route("filter-options/all")]
    public async Task<ActionResult<FilterOptionListData>> GetFilterData()
    {
        var result = await Mediator.Send(new GetFilteringLists.Command());

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<FilterAnalyticsEntries.Results>> Search([FromBody] FilterAnalyticsEntries.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("dashboard")]
    public async Task<ActionResult<AnalyticsDashboard>> GetDashboardData([FromBody] GetDashboardData.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("performance")]
    public async Task<ActionResult<PerformanceDashboard>> GetPerformanceData([FromBody] GetPerformanceData.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("sessions")]
    public async Task<ActionResult<SessionsDashboard>> GetSessionsData([FromBody] GetSessionData.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("delete-matching-path")]
    public async Task<ActionResult<SessionsDashboard>> DeleteMatchingPath([FromBody] DeleteFromPath.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("current-sessions")]
    public async Task<ActionResult<SessionsDashboard>> CurrentSessions([FromBody] GetCurrentSessions.Command data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("fix-api-slash")]
    public async Task<ActionResult> FixApiSlash()
    {
        var result = await Mediator.Send(new NormalizePathsAndActions.Command());

        return HttpResponse(result);
    }
}