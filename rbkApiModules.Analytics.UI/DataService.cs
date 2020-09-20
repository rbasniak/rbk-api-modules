using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using rbkApiModules.Analytics.Core;
using Microsoft.AspNetCore.Http;

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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _url;
        public AnalyticsDataService(IHttpClientFactory clientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientFactory = clientFactory;
            _url = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}{httpContextAccessor.HttpContext.Request.PathBase}";
        }

        public async Task<FilterOptionListData> GetFilterDataAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _url + "/api/analytics/filter-options");
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
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/filter");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

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

        public async Task<SimpleNamedEntity[]> GetMostActiveDomains(GetMostActiveDomains.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-active-domains");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetMostActiveUsers(GetMostActiveUsers.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-active-users");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetMostFailedEndpoints(GetMostFailedEndpoints.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-failed-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetMostUsedReadEndpoints(GetMostUsedReadEndpoints.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-used-read-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetMostUsedWriteEndpoints(GetMostUsedWriteEndpoints.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-used-write-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetSlowestReadEndpoints(GetSlowestReadEndpoints.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/slowest-read-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleNamedEntity[]> GetSlowestWriteEndpoints(GetSlowestWriteEndpoints.Command data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/slowest-write-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleNamedEntity[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }
    }
}
