using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.Models;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core.Controller
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
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostUsedReadEndpoints([FromBody] GetMostUsedEndpoints.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-resource-hungry-endpoints")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostResourceHungryEndpoints([FromBody] GetMostResourceHungryEndpoints.Command data)
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
        [Route("most-active-days")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostActiveDays([FromBody] GetMostActiveDays.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("most-active-hours")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetMostActiveHours([FromBody] GetMostActiveHours.Command data)
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

        [HttpPost]
        [Route("daily-active-users")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyActiveUsers([FromBody] GetDailyActiveUsers.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-errors")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyErrors([FromBody] GetDailyErrors.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-inbound-traffic")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyInboundTraffic([FromBody] GetDailyInboundTraffic.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-outbound-traffic")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyOutboundTraffic([FromBody] GetDailyOutboundTraffic.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-requests")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyRequests([FromBody] GetDailyRequests.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-transactions")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyTransactions([FromBody] GetDailyTransactions.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-database-time")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyDatabaseUsage([FromBody] GetDailyDatabaseUsage.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("daily-authentication-failures")]
        public async Task<ActionResult<DateValuePoint[]>> GetDailyAuthenticationFailures([FromBody] GetDailyAuthenticationFailures.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("average-transactions-per-endpoint")]
        public async Task<ActionResult<DateValuePoint[]>> GetAverageTransactionsPerEndpoint([FromBody] GetAverageTransactionsPerEndpoint.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("total-time-comsumption-per-write-endpoint")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerWriteEndpoint([FromBody] GetTotalTimeComsumptionPerWriteEndpoint.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }

        [HttpPost]
        [Route("total-time-comsumption-per-read-endpoint")]
        public async Task<ActionResult<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerReadEndpoint([FromBody] GetTotalTimeComsumptionPerReadEndpoint.Command data)
        {
            var result = await Mediator.Send(data);

            return HttpResponse(result);
        }
    }
}
