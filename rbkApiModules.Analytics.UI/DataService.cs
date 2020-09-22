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
using Microsoft.AspNetCore.Mvc.Formatters;

namespace rbkApiModules.Analytics.UI
{
    public interface IAnalyticsDataService
    {
        Task<FilterOptionListData> GetFilterDataAsync();
        Task<AnalyticsEntry[]> FilterAsync(FilterAnalyticsEntries.Command data);
        Task<SimpleLabeledValue<int>[]> GetMostActiveDomains(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetMostActiveUsers(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetMostFailedEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetMostUsedReadEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetMostUsedWriteEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetSlowestReadEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetSlowestWriteEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetBiggestResponsesEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<int>[]> GetBiggestRequestsEndpoints(DateTime from, DateTime to);
        Task<SimpleLabeledValue<double>[]> GetCachedRequestsProportion(DateTime from, DateTime to);
        Task<SimpleLabeledValue<double>[]> GetEndpointErrorRates(DateTime from, DateTime to);
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

        public async Task<SimpleLabeledValue<int>[]> GetMostActiveDomains(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-active-domains");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetMostActiveDomains.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetMostActiveUsers(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-active-users");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetMostActiveUsers.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetMostFailedEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-failed-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetMostFailedEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetMostUsedReadEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-used-read-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetMostUsedReadEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetMostUsedWriteEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/most-used-write-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetMostUsedWriteEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetSlowestReadEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/slowest-read-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetSlowestReadEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetSlowestWriteEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/slowest-write-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetSlowestWriteEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetBiggestResponsesEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/biggest-responses-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetBiggestResponsesEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<int>[]> GetBiggestRequestsEndpoints(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/biggest-requests-endpoints");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetBiggestResquestsEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<int>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<double>[]> GetCachedRequestsProportion(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/cached-requests-proportion");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetBiggestResquestsEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<double>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }

        public async Task<SimpleLabeledValue<double>[]> GetEndpointErrorRates(DateTime from, DateTime to)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, _url + "/api/analytics/endpoint-error-rates");
            // request.Headers.Add("User-Agent", "HttpClientFactory-Sample");

            request.Content = new StringContent(JsonConvert.SerializeObject(new GetBiggestResquestsEndpoints.Command(from, to)), Encoding.UTF8, "application/json");

            var client = _clientFactory.CreateClient();

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SimpleLabeledValue<double>[]>(jsonContent);
                return result;
            }
            else
            {
                throw new HttpRequestException();
            }
        }
    }
}
