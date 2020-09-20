using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Controller
{
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
        [Route("most-active-domains")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetMostActiveDomains([FromBody] GetMostActiveDomains.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-active-users")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetMostActiveUsers([FromBody] GetMostActiveUsers.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-failed-endpoints")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetMostFailedEndpoints([FromBody] GetMostFailedEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-used-read-endpoints")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetMostUsedReadEndpoints([FromBody] GetMostUsedReadEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-used-write-endpoints")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetMostUsedWriteEndpoints([FromBody] GetMostUsedWriteEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("slowest-read-endpoints")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetSlowestReadEndpoints([FromBody] GetSlowestReadEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("slowest-write-endpoints")]
        public async Task<ActionResult<SimpleNamedEntity[]>> GetSlowestWriteEndpoints([FromBody] GetSlowestWriteEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
