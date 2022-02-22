using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Infrastructure.Api;

namespace rbkApiModules.Logs.Core
{
    [ApiController]
    [AllowAnonymous]
    [ApplicationArea("logs")]
    [Route("api/[controller]")]
    public class LogController : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Save([FromBody] SaveLogEntry.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpGet]
        [Route("filter-options")]
        public async Task<ActionResult<FilterLogsEntries>> GetFilterData()
        {
            var result = await Mediator.Send(new GetFilteringLists.Command());

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<FilterLogsEntries.Results>> Search([FromBody] FilterLogsEntries.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpPost]
        [Route("dashboard")]
        public async Task<ActionResult<LogsDashboard>> GetDashboardData([FromBody] GetDashboardData.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpPost]
        [Route("delete-old-data")]
        public async Task<ActionResult> DeleteOldData([FromBody] DeletePriorFrom.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
