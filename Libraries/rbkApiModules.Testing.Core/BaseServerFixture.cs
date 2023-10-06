using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using Serilog;
using System.Net.Http.Headers;
using System.Net;

namespace rbkApiModules.Testing.Core;

public class BaseServerFixture : IDisposable
{
    private readonly Dictionary<Credentials, string> _accessTokens = new();
    private readonly string _contentFolder;

    public BaseServerFixture(Type startupClassType, string authenticationMode)
    {
        if (authenticationMode == null) throw new ArgumentNullException(nameof(authenticationMode));

        authenticationMode = authenticationMode.ToLower();

        if (authenticationMode != CredentialsAuthenticationModeName && authenticationMode != WindowsAuthenticationModeName) throw new ArgumentOutOfRangeException(nameof(authenticationMode));

        var projectDir = Path.GetDirectoryName(startupClassType.Assembly.Location);

        _contentFolder = Path.Combine(projectDir, "wwwroot");

        if (Directory.Exists(_contentFolder))
        {
            Directory.Delete(_contentFolder, true);
        }

        if (!Directory.Exists(_contentFolder))
        {
            Directory.CreateDirectory(_contentFolder);
        }

        AuthenticationMode = authenticationMode;

        if (authenticationMode == CredentialsAuthenticationModeName)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Server = new TestServer(new WebHostBuilder()
                .UseWebRoot(Path.Combine(_contentFolder))
                .UseEnvironment("Testing")
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile("appsettings.Testing.json")
                    .Build()
                )
                .UseStartup(startupClassType)
                .UseSerilog(Log.Logger));
#pragma warning restore CS0618 // Type or member is obsolete
        }
        else
        {
#pragma warning disable CS0618 // Type or member is obsolete
            Server = new TestServer(new WebHostBuilder()
                .UseWebRoot(Path.Combine(_contentFolder))
                .UseEnvironment("Testing")
                .UseConfiguration(new ConfigurationBuilder()
                    .SetBasePath(projectDir)
                    .AddJsonFile("appsettings.Testing.json")
                    .Build()
                )
                .UseStartup(startupClassType)
                .ConfigureTestServices(services =>
                {
                })
                .UseSerilog(Log.Logger));
#pragma warning restore CS0618 // Type or member is obsolete
        }

        Server.BaseAddress = new Uri("https://localhost/");
    }

    public TestServer Server { get; private set; }

    public string AuthenticationMode { get; private set; }

    public virtual bool CleanUpTestDatabases => true;

    public virtual string MockedWindowsAuthenticationHeaderName => "UserId";
    public virtual string MockedWindowsAuthenticationSchemeName => "TestScheme";

    internal string WindowsAuthenticationModeName => "windows";
    internal string CredentialsAuthenticationModeName => "credentials";

    public DbContext Context
    {
        get
        {
            var scope = Server.Services.CreateScope();

            var contexts = scope.ServiceProvider.GetService<IEnumerable<DbContext>>();

            if (contexts.Count() > 1)
            {
                return contexts.GetDefaultContext();
            }
            else
            {
                return contexts.First();
            }
        }
    }

    public string ContentFolder => _contentFolder;

    public async Task<string> GetAccessTokenAsync(string username, string password, string tenant)
    {
        var credentials = new Credentials(username, password, tenant);

        if (!_accessTokens.ContainsKey(credentials))
        {
            using (var httpClient = Server.CreateClient())
            {
                var body = new 
                {
                    Username = username,
                    Password = password,
                    Tenant = tenant
                };

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MockedWindowsAuthenticationSchemeName);
                httpClient.DefaultRequestHeaders.Add(MockedWindowsAuthenticationHeaderName, username);

                var response = await httpClient.PostAsync("api/authentication/login", new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw new Exception($"Could not login with the provided credentials: {body.Username}///{body.Password} ({body.Tenant})");
                }

                var responseBodyData = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<JwtResponse>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _accessTokens.Add(credentials, data.AccessToken);
            }
        }

        return _accessTokens[credentials];
    }

    public async Task<string> GetAccessTokenAsync(string username, string tenant)
    {
        var credentials = new Credentials(username, String.Empty, tenant);

        if (!_accessTokens.ContainsKey(credentials))
        {
            using (var httpClient = Server.CreateClient())
            {
                var body = new 
                {
                    Tenant = tenant
                };

                if (AuthenticationMode == WindowsAuthenticationModeName)
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(MockedWindowsAuthenticationSchemeName);
                    httpClient.DefaultRequestHeaders.Add(MockedWindowsAuthenticationHeaderName, username);
                }

                var response = await httpClient.PostAsync("api/authentication/login",
                    new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

                response.EnsureSuccessStatusCode();

                var responseBodyData = await response.Content.ReadAsStringAsync();

                var data = JsonSerializer.Deserialize<JwtResponse>(responseBodyData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                _accessTokens.Add(credentials, data.AccessToken);
            }
        }

        return _accessTokens[credentials];
    }

    public string GetDefaultAccessToken()
    {
        return _accessTokens.First().Value;
    }

    public void Dispose()
    {
        if (CleanUpTestDatabases == true)
        {
            Context.Database.EnsureDeleted();
        }

        Server.Dispose();
    }

    internal class JwtResponse
    {
        public string AccessToken { get; set; }
    }
}