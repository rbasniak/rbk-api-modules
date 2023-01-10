using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;

namespace rbkApiModules.Testing.Core;

public static class HttpCallsExtensions
{
    public async static Task<HttpResponse<TResult>> PostAsync<TResult>(this BaseServerFixture fixture, string url, object body, string token, Dictionary<string, string> additionalHeaders = null)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            if (additionalHeaders != null)
            {
                foreach (var header in additionalHeaders)
                {
                    httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            var response = await httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Data = JsonSerializer.Deserialize<TResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }
    
    public async static Task<HttpResponse> PostAsync(this BaseServerFixture fixture, string url, object body, string token) 
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            var response = await httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
            }
            else
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }

    public static Task<HttpResponse<TResult>> PostAsync<TResult>(this BaseServerFixture fixture, string url, object body, bool authenticated)
        where TResult : class
    {
        var token = authenticated ? fixture.GetDefaultAccessToken() : null;

        return fixture.PostAsync<TResult>(url, body, token);
    }

    public async static Task<HttpResponse<TResult>> PutAsync<TResult>(this BaseServerFixture fixture, string url, object body, string token)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            var response = await httpClient.PutAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                result.Data = JsonSerializer.Deserialize<TResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }

    public static Task<HttpResponse<TResult>> PutAsync<TResult>(this BaseServerFixture fixture, string url, object body, bool authenticated)
        where TResult : class
    {
        var token = authenticated ? fixture.GetDefaultAccessToken() : null;

        return fixture.PutAsync<TResult>(url, body, token);
    }

    public async static Task<HttpResponse<TResult>> GetAsync<TResult>(this BaseServerFixture fixture, string url, string token)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            var response = await httpClient.GetAsync(url);

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                result.Messages = new[] { response.Headers.Location.ToString() };
            }
            else if (response.IsSuccessStatusCode)
            {
                result.Data = JsonSerializer.Deserialize<TResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            else
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }

    public async static Task<HttpResponse> GetAsync(this BaseServerFixture fixture, string url, string token) 
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            var response = await httpClient.GetAsync(url);

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.StatusCode == HttpStatusCode.Redirect)
            {
                result.Messages = new[] { response.Headers.Location.ToString() };
            }
            else if (!response.IsSuccessStatusCode)
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }

    public static Task<HttpResponse<TResult>> GetAsync<TResult>(this BaseServerFixture fixture, string url, bool authenticated)
        where TResult : class
    {
        var token = authenticated ? fixture.GetDefaultAccessToken() : null;

        return fixture.GetAsync<TResult>(url, token);
    }

    public async static Task<HttpResponse> DeleteAsync(this BaseServerFixture fixture, string url, string token)
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (token != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }

            var response = await httpClient.DeleteAsync(url);

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    result.Messages = JsonSerializer.Deserialize<string[]>(responseBodyData);
                }
            }

            return result;
        }
    }

    public static Task<HttpResponse> DeleteAsync(this BaseServerFixture fixture, string url, bool authenticated)
    {
        var token = authenticated ? fixture.GetDefaultAccessToken() : null;

        return fixture.DeleteAsync(url, token);
    }
}