using rbkApiModules.Infrastructure.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using rbkApiModules.Analytics.Core;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Utilities;

namespace rbkApiModules.Analytics.UI
{
    public interface IAnalyticsDataService
    {
        Task<HttpResponse<FilterOptionListData>> GetFilterDataAsync();
        Task<HttpResponse<AnalyticsEntry[]>> FilterAsync(FilterAnalyticsEntries.Command data);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveDomains(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveUsers(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostFailedEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostUsedEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostResourceHungryEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetSlowestReadEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetSlowestWriteEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetBiggestResponsesEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetBiggestRequestsEndpoints(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetAverageTransactionsPerEndpoint(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveDays(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveHours(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<double>[]>> GetCachedRequestsProportion(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<double>[]>> GetEndpointErrorRates(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerWriteEndpoint(DateTime from, DateTime to);
        Task<HttpResponse<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerReadEndpoint(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyActiveUsers(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyErrors(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyInboundTraffic(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyOutboundTraffic(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyAuthenticationFailures(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyRequests(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyTransactions(DateTime from, DateTime to);
        Task<HttpResponse<DateValuePoint[]>> GetDailyDatabaseUsage(DateTime from, DateTime to);
    }

    public class AnalyticsDataService : BaseHttpService, IAnalyticsDataService
    {
        private readonly string _url;
        
        public AnalyticsDataService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor): base(clientFactory)
        {
            _url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
        }

        public async Task<HttpResponse<FilterOptionListData>> GetFilterDataAsync()
        {
            return await SendAsync<FilterOptionListData>(HttpMethod.Get, _url + "/api/analytics/filter-options");
        }

        public async Task<HttpResponse<AnalyticsEntry[]>> FilterAsync(FilterAnalyticsEntries.Command data)
        {
            return await SendAsync<AnalyticsEntry[]>(HttpMethod.Post, _url + "/api/analytics/filter", data); 
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveDomains(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-active-domains", new GetMostActiveDomains.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveUsers(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-active-users", new GetMostActiveUsers.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostFailedEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-failed-endpoints", new GetMostFailedEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetSlowestReadEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/slowest-read-endpoints", new GetSlowestReadEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetSlowestWriteEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/slowest-write-endpoints", new GetSlowestWriteEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetBiggestResponsesEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/biggest-responses-endpoints", new GetBiggestResponsesEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetBiggestRequestsEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/biggest-requests-endpoints", new GetBiggestResquestsEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<double>[]>> GetCachedRequestsProportion(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<double>[]>(HttpMethod.Post, _url + "/api/analytics/cached-requests-proportion", new GetBiggestResquestsEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<double>[]>> GetEndpointErrorRates(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<double>[]>(HttpMethod.Post, _url + "/api/analytics/endpoint-error-rates", new GetBiggestResquestsEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyActiveUsers(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-active-users", new GetDailyActiveUsers.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyErrors(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-errors", new GetDailyErrors.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyInboundTraffic(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-inbound-traffic", new GetDailyInboundTraffic.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyOutboundTraffic(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-outbound-traffic", new GetDailyOutboundTraffic.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyAuthenticationFailures(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-authentication-failures", new GetDailyAuthenticationFailures.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyRequests(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-requests", new GetDailyRequests.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostUsedEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-used-read-endpoints", new GetMostUsedEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostResourceHungryEndpoints(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-resource-hungry-endpoints", new GetMostResourceHungryEndpoints.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetAverageTransactionsPerEndpoint(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/average-transactions-per-endpoint", new GetAverageTransactionsPerEndpoint.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyTransactions(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-transactions", new GetDailyTransactions.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyDatabaseUsage(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/analytics/daily-database-time", new GetDailyDatabaseUsage.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveDays(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-active-days", new GetMostActiveDays.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetMostActiveHours(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/most-active-hours", new GetMostActiveHours.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerWriteEndpoint(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/total-time-comsumption-per-write-endpoint", new GetTotalTimeComsumptionPerWriteEndpoint.Command(from, to));
        }

        public async Task<HttpResponse<SimpleLabeledValue<int>[]>> GetTotalTimeComsumptionPerReadEndpoint(DateTime from, DateTime to)
        {
            return await SendAsync<SimpleLabeledValue<int>[]>(HttpMethod.Post, _url + "/api/analytics/total-time-comsumption-per-read-endpoint", new GetTotalTimeComsumptionPerReadEndpoint.Command(from, to));
        }
    }
}
