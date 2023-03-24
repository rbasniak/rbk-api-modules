using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational.CQRS;
using rbkApiModules.Commons.Relational;
using Serilog;

namespace rbkApiModules.Testing.Core;

public class BaseServerFixture : IDisposable
{
    private readonly Dictionary<Credentials, string> _accessTokens = new();
    private readonly string _contentFolder;

    public BaseServerFixture(Type startupClassType, AuthenticationMode authenticationMode)
    {
        var projectDir = Path.GetDirectoryName(startupClassType.Assembly.Location);

        _contentFolder = projectDir;

        AuthenticationMode = authenticationMode;

        Server = new TestServer(new WebHostBuilder()
            .UseContentRoot(_contentFolder)
            .UseConfiguration(new ConfigurationBuilder()
                .SetBasePath(projectDir)
                .AddJsonFile("appsettings.Testing.json")
                .Build()
            )
            .UseStartup(startupClassType)
            .UseSerilog());
    }

    public TestServer Server { get; private set; }

    public AuthenticationMode AuthenticationMode { get; private set; }

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
                var body = new UserLogin.Request
                {
                    Username = username,
                    Password = password,
                    Tenant = tenant
                };

                var response = await httpClient.PostAsync("api/authentication/login", new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json"));

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
        Server.Dispose();
    }
}

