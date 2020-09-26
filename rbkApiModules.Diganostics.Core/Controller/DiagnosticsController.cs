using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : BaseController
    {
        [HttpGet]
        [ApplicationArea("diagnostics")]
        public async Task<ActionResult<DiagnosticsEntry[]>> Get()
        {
            var result = await Mediator.Send(new GetDiagnosticsData.Command());

            return HttpResponse(result);
        }

        [HttpPost]
        [ApplicationArea("diagnostics")]
        public async Task<ActionResult> Filter([FromBody] SaveDiagnosticsEntry.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
