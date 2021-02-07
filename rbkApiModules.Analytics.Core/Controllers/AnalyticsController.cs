using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    [ApplicationArea("analytics")]
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class AnalyticsController: BaseController
    {
        [HttpGet]
        [Route("filter-options")]
        public async Task<ActionResult<FilterAnalyticsEntries>> GetFilterData()
        {
            var result = await Mediator.Send(new GetFilteringLists.Command());

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("filter")]
        public async Task<ActionResult<FilterAnalyticsEntries>> Filter([FromBody] FilterAnalyticsEntries.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("dashboard")]
        public async Task<ActionResult<AnalyticsResults>> GetDashboardData([FromBody] GetDashboardData.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
