using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    [ExcludeFromCodeCoverage]
    [IgnoreOnCodeGeneration]
    [ApplicationArea("analytics")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController: BaseController
    {
        [HttpGet]
        [Route("filter-options")]
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
    }
}
