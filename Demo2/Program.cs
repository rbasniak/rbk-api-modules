using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core.Helpers;
using Demo2.Endpoints;
using Demo2.Clients;

namespace Demo2;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string connectionString;

        if (TestingEnvironmentChecker.IsTestingEnvironment)
        {
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", $"Testing.{Guid.NewGuid():N}");
        }
        else
        {
            connectionString = builder.Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Application");
        }

        builder.Services.AddDbContext<DatabaseContext>((scope, options) => options
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
        );

        builder.Services.AddRbkApiCoreSetup(options => options
            .EnableBasicAuthenticationHandler()
            .UseDefaultCompression()
            .UseDefaultCors()
            .UseDefaultHsts(builder.Environment.IsDevelopment())
            .UseDefaultHttpsRedirection()
            .UseDefaultMemoryCache()
            .UseDefaultHttpClient()
            .UseHttpContextAccessor()
            .UseStaticFiles()
            .RegisterDbContext<DatabaseContext>()
        );

        builder.Services.AddRbkRelationalAuthentication(builder.Configuration, options => options
            .UseSymetricEncryptationKey()
            .UseLoginWithWindowsAuthentication()
            .UseMockedWindowsAuthentication()
            .AddApiKeyAuthentication() 
        );

        builder.Services.AddRbkUIDefinitions(Assembly.GetAssembly(typeof(Program)));

        builder.Services.AddOpenApi();

        builder.Services.AddHttpClient<IDemo1ApiClient, Demo1ApiClient>(nameof(IDemo1ApiClient))
            .ConfigureHttpClient((sp, client) =>
            {
                var baseUrl = sp.GetRequiredService<IConfiguration>()["Demo1Api:BaseUrl"]
                    ?? throw new InvalidOperationException("Demo1Api:BaseUrl is not configured.");
                client.BaseAddress = new Uri(baseUrl);
            });

        var app = builder.Build();

        app.UseRbkApiCoreSetup();

        app.UseRbkRelationalAuthentication();

        app.SetupDatabase<DatabaseContext>(options => options
            .MigrateOnStartup()
        );

        app.SetupRbkAuthenticationClaims(options => options
            .WithCustomDescription(x => x.ChangeClaimProtection, "Change claim protection")
            .WithCustomDescription(x => x.ManageClaims, "Manage application claims")
            .WithCustomDescription(x => x.ManageTenantSpecificRoles, "Manage tenant roles")
            .WithCustomDescription(x => x.ManageApplicationWideRoles, "Manage application roles")
            .WithCustomDescription(x => x.ManageTenants, "Manage tenants")
            .WithCustomDescription(x => x.ManageUsers, "Manage users")
            .WithCustomDescription(x => x.ManageUserRoles, "Manage user roles")
            .WithCustomDescription(x => x.OverrideUserClaims, "Override user claims")
            .WithCustomDescription(x => x.ManageApiKeys, "Manage API keys")
            .WithCustomDescription(x => x.ManageCrossTenantApiKeys, "Create and manage cross-tenant API keys")
        );

        app.SetupRbkDefaultAdmin(options => options
            .WithUsername("superuser")
            .WithPassword("admin")
            .WithDisplayName("Administrator")
            .WithEmail("admin@my-company.com")
        );

        app.SeedDatabase<DatabaseSeed>();

        app.UseRbkUIDefinitions();

        DemoEndpoints.MapEndpoint(app);
        IntegrationEndpoints.MapEndpoint(app);

        app.MapOpenApi().AllowAnonymous();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/openapi/v1.json", "Demo 2");
        });

        app.Run();
    }
}
