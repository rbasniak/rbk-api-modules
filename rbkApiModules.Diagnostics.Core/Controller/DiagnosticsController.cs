using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System;
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

        [HttpPost("filter")]
        public async Task<ActionResult<DiagnosticsEntry[]>> Get(FilterDiagnosticsEntries.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        public async Task<ActionResult<FilterOptionListData>> Filter([FromBody] SaveDiagnosticsEntry.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }       
        
        [HttpPost("daily-area-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailyAreaErrors(GetDailyAreaErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-browser-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailyBrowserErrors(GetDailyBrowserErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-device-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailyDeviceErrors(GetDailyDeviceErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-layer-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailyLayerErrors(GetDailyLayerErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-operating-system-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailyOperatingSystemErrors(GetDailyOperatingSystemErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-source-errors")]
        public async Task<ActionResult<LineChartSeries[]>> GetDailySourceErrors(GetDailySourceErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost("daily-errors")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyErrors(GetDailyErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
