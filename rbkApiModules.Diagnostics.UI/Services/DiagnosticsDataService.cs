using rbkApiModules.Infrastructure.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Utilities;
using rbkApiModules.Diagnostics.Core;
using rbkApiModules.Diagnostics.Commons;

namespace rbkApiModules.Diagnostics.UI
{
    public interface IDiagnosticsDataService
    {
        Task<HttpResponse<FilterOptionListData>> GetFilterDataAsync();
        Task<HttpResponse<DiagnosticsEntry[]>> FilterAsync(FilterDiagnosticsEntries.Command data);
        Task<HttpResponse<DateValuePoint[]>> GetDailyErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailyLayerErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailyAreaErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailyBrowserErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailyDeviceErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailyOperatingSystemErrors(DateTime from, DateTime to);
        Task<HttpResponse<LineChartSeries[]>> GetDailySourceErrors(DateTime from, DateTime to); 
    }

    public class DiagnosticsDataService : BaseHttpService, IDiagnosticsDataService
    {
        private readonly string _url;
        
        public DiagnosticsDataService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor): base(clientFactory)
        {
            _url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
        }

        public async Task<HttpResponse<DiagnosticsEntry[]>> FilterAsync(FilterDiagnosticsEntries.Command data)
        {
            return await SendAsync<DiagnosticsEntry[]>(HttpMethod.Post, _url + "/api/diagnostics/filter", data);
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailyAreaErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]> (HttpMethod.Post, _url + "/api/diagnostics/daily-area-errors", new GetDailyAreaErrors.Command(from, to));
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailyBrowserErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-browser-errors", new GetDailyBrowserErrors.Command(from, to));
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailyDeviceErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-device-errors", new GetDailyDeviceErrors.Command(from, to));
        }

        public async Task<HttpResponse<DateValuePoint[]>> GetDailyErrors(DateTime from, DateTime to)
        {
            return await SendAsync<DateValuePoint[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-errors", new GetDailyErrors.Command(from, to));
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailyLayerErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-layer-errors", new GetDailyLayerErrors.Command(from, to));
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailyOperatingSystemErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-operating-system-errors", new GetDailyOperatingSystemErrors.Command(from, to));
        }

        public async Task<HttpResponse<LineChartSeries[]>> GetDailySourceErrors(DateTime from, DateTime to)
        {
            return await SendAsync<LineChartSeries[]>(HttpMethod.Post, _url + "/api/diagnostics/daily-source-errors", new GetDailySourceErrors.Command(from, to));
        }

        public async Task<HttpResponse<FilterOptionListData>> GetFilterDataAsync()
        {
            return await SendAsync<FilterOptionListData>(HttpMethod.Get, _url + "/api/diagnostics/filter-options");
        }
    }
}
