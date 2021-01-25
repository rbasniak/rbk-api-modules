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
        Task<HttpResponse<AnalyticsResults>> GetDashboardData(DateTime from, DateTime to);
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

        public async Task<HttpResponse<AnalyticsResults>> GetDashboardData(DateTime from, DateTime to)
        {
            return await SendAsync<AnalyticsResults>(HttpMethod.Post, _url + "/api/analytics/dashboard", new GetDashboardData.Command(from, to));
        }

    }
}
