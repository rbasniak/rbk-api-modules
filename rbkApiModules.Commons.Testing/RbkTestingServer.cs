using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using Shouldly;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using TUnit.Core.Interfaces;

namespace rbkApiModules.Commons.Testing;

public abstract class RbkTestingServer<TProgram> : WebApplicationFactory<TProgram>, IAsyncInitializer where TProgram : class
{
    public string ContentFolder { get; private set; } = string.Empty;

    private readonly Dictionary<Type, Mock<HttpMessageHandler>> _mockedHttpClientMessageHandlers = new();

    protected readonly Dictionary<Credentials, string> CachedCredentials = new();

    public string InstanceId = Guid.NewGuid().ToString("N");

    private TestServer? TestingServer;

    private const string MockedWindowsAuthenticationHeaderName = "UserId";
    private const string MockedWindowsAuthenticationSchemeName = "TestScheme";

    public string LoginUrl { get; set; } = "api/authentication/login";

    protected abstract bool UseHttps { get; }

    public async Task InitializeAsync()
    {
        Debug.WriteLine($"*** RbkTestingServer Initialize: {InstanceId}");

        await InitializeApplicationAsync();

        // Grab a reference to the server
        // This forces it to initialize.
        // By doing it within this method, it's thread safe.
        // And avoids multiple initialisations from different tests if parallelisation is switched on
        TestingServer = Server;

    }

    protected abstract Task InitializeApplicationAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var projectDir = Path.GetDirectoryName(typeof(TProgram).Assembly.Location);

        ContentFolder = Path.Combine(projectDir, $"wwwroot_{Guid.NewGuid().ToString("N")}");

        if (Directory.Exists(ContentFolder))
        {
            Directory.Delete(ContentFolder, true);
        }

        Directory.CreateDirectory(ContentFolder);

        builder
            .UseEnvironment("Testing")
            .UseConfiguration(
                new ConfigurationBuilder()
                    .SetBasePath(projectDir!)
                    .AddJsonFile("appsettings.Testing.json", optional: false, reloadOnChange: false)
                    .AddInMemoryCollection(ConfigureInMemoryOverrides()) // last wins
                    .Build()
            )
            .UseWebRoot(ContentFolder)
            //.UseConfiguration(new ConfigurationBuilder()
            //    .SetBasePath(projectDir!)
            //    .AddJsonFile("appsettings.Testing.json")
            //    .Build()
            //)
            .ConfigureTestServices(services => ConfigureTestServices(services));

        base.ConfigureWebHost(builder);
    }

    protected abstract IEnumerable<KeyValuePair<string, string>> ConfigureInMemoryOverrides();

    protected abstract void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder config);

    protected abstract void ConfigureTestServices(IServiceCollection services);

    private HttpClient CreateHttpClient()
    {
        var client = Server.CreateClient();
        client.BaseAddress = new Uri($"{(UseHttps ? "https" : "http")}://test-server.com/");
        return client;
    }

    public IDispatcher Dispatcher
    {
        get
        {
            var scope = TestingServer!.Services.CreateScope();
            var dispatcher = scope.ServiceProvider.GetRequiredService<IDispatcher>();
            return dispatcher;
        }
    }

    public virtual DbContext CreateContext()
    {
        var scope = TestingServer!.Services.CreateScope();

        var contexts = scope.ServiceProvider.GetService<IEnumerable<DbContext>>();

        if (contexts != null && contexts.Count() > 1)
        {
            var context = contexts.GetDefaultContext();

            return context ?? throw new Exception("Could not find DbContext");
        }
        else
        {
            if (contexts == null)
            {
                throw new Exception("Could not find a DbContext in the DI container");
            }

            var context = contexts.First();

            return context ?? throw new Exception("Could not find DbContext");
        }
    }

    // Does NOT support multiple integration tests running in parallel
    public void AddMockHttpClient<TClient, TImplementation>(IServiceCollection services, string name)
        where TClient : class
        where TImplementation : class, TClient
    {
        var handler = new Mock<HttpMessageHandler>();
        _mockedHttpClientMessageHandlers[typeof(TClient)] = handler;

        var client = new HttpClient(handler.Object)
        {
            BaseAddress = new Uri($"{(UseHttps ? "https" : "http")}://test-server.com/")
        };

        services.AddHttpClient<TClient, TImplementation>(name)
                .ConfigurePrimaryHttpMessageHandler(() => handler.Object);
    }

    public Mock<HttpMessageHandler> GetMockedHttpClientMessageHandler<TClient>()
    {
        return _mockedHttpClientMessageHandlers[typeof(TClient)];
    }

    #region Post

    public async Task<HttpResponse> PostAsync(string url, object body, ApiKey credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await PostAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse> PostAsync(string url, object body, Credentials credentials)
    {
        return await PostAsync(url, body, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse> PostAsync(string url, object body, string username)
    {
        return await PostAsync(url, body, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse> PostAsync(string url, object body, JwtToken credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await PostAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse> PostAsync(string url, object body)
    {
        using (var httpClient = CreateHttpClient())
        {
            return await PostAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body, ApiKey credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await PostAsync<TResponse>(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body, Credentials credentials) where TResponse : class
    {
        return await PostAsync<TResponse>(url, body, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body, string username) where TResponse : class
    {
        return await PostAsync<TResponse>(url, body, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body, JwtToken credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await PostAsync<TResponse>(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            return await PostAsync<TResponse>(httpClient, url, body);
        }
    }

    private async Task<HttpResponse> PostAsync(HttpClient httpClient, string url, object body)
    {
        var response = await httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        var result = await ProcessResponse(response);

        return result;
    }

    private async Task<HttpResponse<TResponse>> PostAsync<TResponse>(HttpClient httpClient, string url, object body) where TResponse : class
    {
        var response = await httpClient.PostAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        var result = await ProcessResponse<TResponse>(response);

        return result;
    }

    #endregion

    #region Put

    public async Task<HttpResponse> PutAsync(string url, object body, ApiKey credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await PutAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse> PutAsync(string url, object body, Credentials credentials)
    {
        return await PutAsync(url, body, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse> PutAsync(string url, object body, string username)
    {
        return await PutAsync(url, body, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse> PutAsync(string url, object body, JwtToken credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await PutAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse> PutAsync(string url, object body)
    {
        using (var httpClient = CreateHttpClient())
        {
            return await PutAsync(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, ApiKey credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await PutAsync<TResponse>(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, Credentials credentials) where TResponse : class
    {
        return await PutAsync<TResponse>(url, body, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, string username) where TResponse : class
    {
        return await PutAsync<TResponse>(url, body, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, JwtToken credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await PutAsync<TResponse>(httpClient, url, body);
        }
    }

    public async Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            return await PutAsync<TResponse>(httpClient, url, body);
        }
    }

    private async Task<HttpResponse> PutAsync(HttpClient httpClient, string url, object body)
    {
        var response = await httpClient.PutAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        var result = await ProcessResponse(response);

        return result;
    }
    private async Task<HttpResponse<TResponse>> PutAsync<TResponse>(HttpClient httpClient, string url, object body) where TResponse : class
    {
        var response = await httpClient.PutAsync(url, new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

        var result = await ProcessResponse<TResponse>(response);

        return result;
    }

    #endregion

    #region Delete

    public async Task<HttpResponse> DeleteAsync(string url, ApiKey credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await DeleteAsync(httpClient, url);
        }
    }

    public async Task<HttpResponse> DeleteAsync(string url, Credentials credentials)
    {
        return await DeleteAsync(url, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse> DeleteAsync(string url, string username)
    {
        return await DeleteAsync(url, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse> DeleteAsync(string url, JwtToken credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await DeleteAsync(httpClient, url);
        }
    }

    public async Task<HttpResponse> DeleteAsync(string url)
    {
        using (var httpClient = CreateHttpClient())
        {
            return await DeleteAsync(httpClient, url);
        }
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, ApiKey credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await DeleteAsync<TResponse>(httpClient, url);
        }
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, Credentials credentials) where TResponse : class
    {
        return await DeleteAsync<TResponse>(url, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, string username) where TResponse : class
    {
        return await DeleteAsync<TResponse>(url, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, JwtToken credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await DeleteAsync<TResponse>(httpClient, url);
        }
    }

    public async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            return await DeleteAsync<TResponse>(httpClient, url);
        }
    }

    private async Task<HttpResponse> DeleteAsync(HttpClient httpClient, string url)
    {
        var response = await httpClient.DeleteAsync(url);

        var result = await ProcessResponse(response);

        return result;
    }

    private async Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(HttpClient httpClient, string url) where TResponse : class
    {
        var response = await httpClient.DeleteAsync(url);

        var result = await ProcessResponse<TResponse>(response);

        return result;
    }

    #endregion

    #region Get

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, ApiKey credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await GetAsync<TResponse>(httpClient, url);
        }
    }

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, Credentials credentials) where TResponse : class
    {
        return await GetAsync<TResponse>(url, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, string username) where TResponse : class
    {
        return await GetAsync<TResponse>(url, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, JwtToken credentials) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await GetAsync<TResponse>(httpClient, url);
        }
    }

    public async Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url) where TResponse : class
    {
        using (var httpClient = CreateHttpClient())
        {
            return await GetAsync<TResponse>(httpClient, url);
        }
    }

    public async Task<HttpResponse> GetAsync(string url, ApiKey credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, credentials.Value);

            return await GetAsync(httpClient, url);
        }
    }

    public async Task<HttpResponse> GetAsync(string url, Credentials credentials)
    {
        return await GetAsync(url, GetCredentialsFromCache(credentials));
    }

    public async Task<HttpResponse> GetAsync(string url, string username)
    {
        return await GetAsync(url, GetCredentialsFromCache(username));
    }

    public async Task<HttpResponse> GetAsync(string url, JwtToken credentials)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", credentials.Value);

            return await GetAsync(httpClient, url);
        }
    }

    public async Task<HttpResponse> GetAsync(string url)
    {
        using (var httpClient = CreateHttpClient())
        {
            return await GetAsync(httpClient, url);
        }
    }

    private async Task<HttpResponse<TResponse>> GetAsync<TResponse>(HttpClient httpClient, string url) where TResponse : class
    {
        var response = await httpClient.GetAsync(url);

        var result = await ProcessResponse<TResponse>(response);

        return result;
    }

    private async Task<HttpResponse> GetAsync(HttpClient httpClient, string url)
    {
        var response = await httpClient.GetAsync(url);

        var result = await ProcessResponse(response);

        return result;
    }


    #endregion

    #region Login

    /// <summary>
    /// Login using Mocked Windows Authentication
    /// </summary>
    public virtual async Task CacheCredentialsAsync(string username, string tenant)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MockedWindowsAuthenticationSchemeName);
            httpClient.DefaultRequestHeaders.Add(MockedWindowsAuthenticationHeaderName, username);

            var result = await PostAsync<JwtResponse>(httpClient, LoginUrl, new UserLogin.Request
            {
                Username = username,
                Tenant = tenant
            });

            if (!result.IsSuccess)
            {
                var exception = new InvalidOperationException($"Could not login with user {username} (Status Code = {result.Code}). Messages: {String.Join(", ", result.Messages)}");
                exception.Data.Add("Details", result);
                throw exception;
            }

            CachedCredentials.Add(new Credentials(username, string.Empty, tenant), result.Data.AccessToken);
        }
    }

    public virtual async Task<HttpResponse<JwtResponse>> LoginAsync(string username, string? tenant)
    {
        using (var httpClient = CreateHttpClient())
        {
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MockedWindowsAuthenticationSchemeName);
            httpClient.DefaultRequestHeaders.Add(MockedWindowsAuthenticationHeaderName, username);

            var result = await PostAsync<JwtResponse>(httpClient, LoginUrl, new UserLogin.Request
            {
                Username = username,
                Tenant = tenant
            });

            return result;
        }
    }

    /// <summary>
    /// Login using normal credentials
    /// </summary>
    public virtual async Task CacheCredentialsAsync(string username, string password, string? tenant)
    {
        using (var httpClient = CreateHttpClient())
        {
            var result = await PostAsync<JwtResponse>(httpClient, LoginUrl, new UserLogin.Request
            {
                Username = username,
                Password = password,
                Tenant = tenant
            });

            if (!result.IsSuccess)
            {
                var exception = new InvalidOperationException($"Could not login with user {username} (Status Code = {result.Code}). Messages: {String.Join(", ", result.Messages)}");
                exception.Data.Add("Details", result);
                throw exception;
            }

            CachedCredentials.Add(new Credentials(username, password, tenant), result.Data.AccessToken);
        }
    }

    public virtual async Task<HttpResponse<JwtResponse>> LoginAsync(string username, string password, string? tenant)
    {
        using (var httpClient = CreateHttpClient())
        {
            var result = await PostAsync<JwtResponse>(httpClient, LoginUrl, new UserLogin.Request
            {
                Username = username,
                Password = password,
                Tenant = tenant
            });

            return result;
        }
    }

    #endregion

    private JwtToken GetCredentialsFromCache(Credentials credentials)
    {
        if (!CachedCredentials.TryGetValue(credentials, out var accessToken))
        {
            throw new InvalidOperationException($"Credentials for user {credentials.Username} in tenant {credentials.Tenant} not found in cache, you need to login first");
        }

        return new JwtToken(accessToken);
    }

    private JwtToken GetCredentialsFromCache(string username)
    {
        var credentials = CachedCredentials.Where(x => x.Key.Username == username).ToArray();

        if (credentials.Length == 0)
        {
            throw new InvalidOperationException($"No credentials found for user {username}, you need to login first");
        }

        if (credentials.Length > 1)
        {
            throw new InvalidOperationException($"Multiple credentials found for user {username}, you need to use the overload with full credentials");
        }

        return new JwtToken(credentials[0].Value);
    }

    private async Task<HttpResponse> ProcessResponse(HttpResponseMessage response)
    {
        var result = new HttpResponse();

        result.Code = response.StatusCode;
        result.Body = await response.Content.ReadAsStringAsync();

        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            result.Messages = [response.Headers.Location.ToString()];
        }
        else if (response.IsSuccessStatusCode)
        {
            // Request without body, don't do anything;
        }
        else
        {
            DeserializeErrorResult(result);
        }

        return result;
    }

    private async Task<HttpResponse<TResponse>> ProcessResponse<TResponse>(HttpResponseMessage response) where TResponse : class
    {
        var result = new HttpResponse<TResponse>();

        result.Code = response.StatusCode;
        result.Body = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            try
            {
                result.Data = JsonSerializer.Deserialize<TResponse>(result.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                result.Data.ShouldNotBeNull($"Error deserializing the response body to {typeof(TResponse).Name}: {ex.Message}");
            }

            result.Data.ShouldNotBeNull($"Error deserializing the response body to {typeof(TResponse).Name}");
        }
        else
        {
            DeserializeErrorResult(result);
        }

        return result;
    }

    private void DeserializeErrorResult(HttpResponse result)
    {
        result.IsSuccess.ShouldBeFalse("Successful response do not have error messages");

        if (!String.IsNullOrEmpty(result.Body))
        {
            // ValidationProblemDetails inherits from ProblemDetails, and we cannot differentiate between the two
            // so we need always deserialize as ValidationProblemDetails 
            var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(result.Body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            result.Problem = problem;

            if (problem != null && problem.Errors != null && problem.Errors.Count > 0)
            {
                result.Messages = problem.Errors.SelectMany(x => x.Value).ToArray();
            }
        }
    }

    public string GetResourceFile(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        var path = Path.Combine(Directory.GetCurrentDirectory(), "Resources", filename);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Resource file not found", path);
        }

        byte[] bytes;
        try
        {
            bytes = File.ReadAllBytes(path);
        }
        catch (Exception ex)
        {
            throw new IOException("Error reading the resource file", ex);
        }

        return Convert.ToBase64String(bytes);
    }

    public MultipartFormDataContent CreateMultipartFormDataContent(Stream fileStream, string formFieldName, string fileName, string contentType)
    {
        var data = new MultipartFormDataContent();
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = formFieldName,
            FileName = fileName
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
        data.Add(fileContent, formFieldName, fileName);

        return data;
    }

    public override async ValueTask DisposeAsync()
    {
        await base.DisposeAsync();

        try
        {
            if (Directory.Exists(ContentFolder))
            {
                Directory.Delete(ContentFolder, true);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"*** RbkTestingServer Dispose: Could not delete content folder {ContentFolder}. Exception: {ex.Message}");
        }
    }
}

public record Credentials
{
    public Credentials(string username, string password, string tenant)
    {
        Username = username;
        Password = password;
        Tenant = tenant;
    }

    public Credentials(string username, string tenant)
    {
        Username = username;
        Password = string.Empty;
        Tenant = tenant;
    }

    public string Username { get; init; }
    public string Password { get; init; }
    public string Tenant { get; init; }
};
