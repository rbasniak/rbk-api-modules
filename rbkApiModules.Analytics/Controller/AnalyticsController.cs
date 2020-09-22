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
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostActiveDomains([FromBody] GetMostActiveDomains.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-active-users")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostActiveUsers([FromBody] GetMostActiveUsers.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-failed-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostFailedEndpoints([FromBody] GetMostFailedEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-used-read-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostUsedReadEndpoints([FromBody] GetMostUsedReadEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-used-write-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostUsedWriteEndpoints([FromBody] GetMostUsedWriteEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("slowest-read-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetSlowestReadEndpoints([FromBody] GetSlowestReadEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("slowest-write-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetSlowestWriteEndpoints([FromBody] GetSlowestWriteEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("biggest-requests-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetBiggestResquestsEndpoints([FromBody] GetBiggestResquestsEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("biggest-responses-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetBiggestResponsesEndpoints([FromBody] GetBiggestResponsesEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("cached-requests-proportion")]
        public async Task<ActionResult<SimpleLabeledValue<double>[]>> GetCachedRequestsProportion([FromBody] GetCachedRequestsProportion.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("endpoint-error-rates")]
        public async Task<ActionResult<SimpleLabeledValue<double>[]>> GetEndpointErrorRates([FromBody] GetEndpointErrorRates.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
