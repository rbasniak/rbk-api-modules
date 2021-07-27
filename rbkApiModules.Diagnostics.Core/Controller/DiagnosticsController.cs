using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.CodeGeneration.Commons;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    [ApiController]
    [AllowAnonymous]
    [ApplicationArea("diagnostics")]
    [Route("api/[controller]")]
    public class DiagnosticsController : BaseController
    {
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Save([FromBody] SaveDiagnosticsEntry.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpGet]
        [Route("filter-options")]
        public async Task<ActionResult<FilterDiagnosticsEntries>> GetFilterData()
        {
            var result = await Mediator.Send(new GetFilteringLists.Command());

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpPost]
        [Route("search")]
        public async Task<ActionResult<FilterDiagnosticsEntries.Results>> Search([FromBody] FilterDiagnosticsEntries.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [IgnoreOnCodeGeneration]
        [HttpPost]
        [Route("dashboard")]
        public async Task<ActionResult<DiagnosticsDashboard>> GetDashboardData([FromBody] GetDashboardData.Command data)
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
