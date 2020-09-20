using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using rbkApiModules.Analytics.Core;

namespace rbkApiModules.Analytics.UI
{
    public interface IAnalyticsDataService
    {
        Task<FilterOptionListData> GetFilterDataAsync();
        Task<AnalyticsEntry[]> FilterAsync(FilterAnalyticsEntries.Command data);
    }

    public class AnalyticsDataService : IAnalyticsDataService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly string _url = "https://localhost:44339";
        public AnalyticsDataService(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<FilterOptionListData> GetFilterDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/analytics/filter-data");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FilterOptionListData>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            } 
        }

        public async Task<AnalyticsEntry[]> FilterAsync(FilterAnalyticsEntries.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/analytics/filter");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<AnalyticsEntry[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }
    }
}
