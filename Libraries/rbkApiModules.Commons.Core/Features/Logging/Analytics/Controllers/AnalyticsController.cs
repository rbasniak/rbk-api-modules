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

[IgnoreOnCodeGeneration]
[ApiController]
[Route("api/[controller]")]
public class AnalyticsController : BaseController
{
    [HttpGet]
    [Route("filter-options/all")]
    public async Task<ActionResult<FilterOptionListData>> GetFilterData()
    {
        var result = await Mediator.Send(new GetFilteringLists.Request());

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("search")]
    public async Task<ActionResult<FilterAnalyticsEntries.Results>> Search([FromBody] FilterAnalyticsEntries.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("dashboard")]
    public async Task<ActionResult<AnalyticsDashboard>> GetDashboardData([FromBody] GetDashboardData.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("performance")]
    public async Task<ActionResult<PerformanceDashboard>> GetPerformanceData([FromBody] GetPerformanceData.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }

    [HttpPost]
    [Route("sessions")]
    public async Task<ActionResult<SessionsDashboard>> GetSessionsData([FromBody] GetSessionData.Request data)
    {
        var result = await Mediator.Send(data);

        return HttpResponse(result);
    }
}