using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Testing.Core;

public static class HttpCallsExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="fixture"></param>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="credentials">Pass the JWT token if not using Windows Authentication, otherwise, simply pass the username</param>
    /// <param name="additionalHeaders"></param>
    /// <returns></returns>
    public async static Task<HttpResponse<TResult>> PostAsync<TResult>(this BaseServerFixture fixture, string url, object body, string credentials, Dictionary<string, string> additionalHeaders = null)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (fixture.AuthenticationMode == fixture.CredentialsAuthenticationModeName || url != "api/authentication/login")
            {
                if (credentials != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
                }

                if (additionalHeaders != null)
                {
                    foreach (var header in additionalHeaders)
                    {
                        httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(fixture.MockedWindowsAuthenticationSchemeName);
                httpClient.DefaultRequestHeaders.Add(fixture.MockedWindowsAuthenticationHeaderName, credentials);
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
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
                }
            }

            return result;
        }
    }

    public async static Task<HttpResponse> PostAsync(this BaseServerFixture fixture, string url, object body, string credentials)
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (fixture.AuthenticationMode == fixture.CredentialsAuthenticationModeName || url != "api/authentication/login")
            {
                if (credentials != null)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
                }
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(fixture.MockedWindowsAuthenticationSchemeName);
                httpClient.DefaultRequestHeaders.Add(fixture.MockedWindowsAuthenticationHeaderName, credentials);
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
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
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

    public async static Task<HttpResponse<TResult>> PutAsync<TResult>(this BaseServerFixture fixture, string url, object body, string credentials)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (credentials != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
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
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
                }
            }

            return result;
        }
    }

    public async static Task<HttpResponse> PutAsync(this BaseServerFixture fixture, string url, object body, string credentials)
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (credentials != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
            }

            var response = await httpClient.PutAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
            }
            else
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
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

    public async static Task<HttpResponse<TResult>> GetAsync<TResult>(this BaseServerFixture fixture, string url, string credentials)
        where TResult : class
    {
        var result = new HttpResponse<TResult>();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (credentials != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
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
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
                }
            }

            return result;
        }
    }

    public async static Task<HttpResponse> GetAsync(this BaseServerFixture fixture, string url, string credentials)
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (credentials != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
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
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
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

    public async static Task<HttpResponse> DeleteAsync(this BaseServerFixture fixture, string url, string credentials)
    {
        var result = new HttpResponse();

        using (var httpClient = fixture.Server.CreateClient())
        {
            if (credentials != null)
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials);
            }

            var response = await httpClient.DeleteAsync(url);

            result.Code = response.StatusCode;

            var responseBodyData = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (!String.IsNullOrEmpty(responseBodyData))
                {
                    var errorResult = JsonSerializer.Deserialize<ErrorResult>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (errorResult != null)
                    {
                        result.Messages = errorResult.Errors;
                    }
                    else
                    {
                        result.Messages = new string[] { responseBodyData };
                    }
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