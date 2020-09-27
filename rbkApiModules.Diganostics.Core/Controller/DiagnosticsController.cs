using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    [AllowAnonymous]
    [ApiController]
    [ApplicationArea("diagnostics")]
    [Route("api/[controller]")]
    public class DiagnosticsController : BaseController
    {
        [HttpGet]
        [Route("filter-options")]
        public async Task<ActionResult<FilterDiagnosticsEntries>> GetFilterData()
        {
            var result = await Mediator.Send(new GetFilteringLists.Command());

            return HttpResponse(result);
        }

        [HttpGet]
        public async Task<ActionResult<DiagnosticsEntry[]>> Get()
        {
            var result = await Mediator.Send(new FilterDiagnosticsEntries.Command());

            return HttpResponse(result);
        }

        [HttpPost]
        public async Task<ActionResult> Filter([FromBody] SaveDiagnosticsEntry.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
