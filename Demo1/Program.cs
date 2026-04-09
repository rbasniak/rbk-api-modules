using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core.Helpers;
using Demo1.Endpoints;
using Demo1.UseCases.Commands;

namespace Demo1
{
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
                 // .UseDefaultSwagger("PoC for the new API libraries")
                 .UseHttpContextAccessor()
                 .UseStaticFiles()
                 .RegisterDbContext<DatabaseContext>()
             );

            builder.Services.AddApplicationOptions<MySettings1>();
            builder.Services.AddApplicationOptions<MySettings2>();
            builder.Services.AddApplicationOptionsManager();

            builder.Services.AddRbkRelationalAuthentication(options => options
                .UseSymetricEncryptationKey()
                .AllowUserCreationByAdmins()
                .AllowUserSelfRegistration()
                .AddApiKeyAuthentication()
            );

            builder.Services.AddRbkUIDefinitions(Assembly.GetAssembly(typeof(Program)));

            // Deferred seed runs after startup; use for steps that depend on events/projections (e.g. projects seed).
            // Add steps with: AddDeferredSeedRunner(options => options.AddDeferredSeed<DatabaseContext>("id", Method, EnvironmentUsage.Development));
            builder.Services.AddDeferredSeedRunner();

            builder.Services.AddOpenApi();

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
                .WithCustomDescription(x => x.CreateCrossTenantApiKeys, "Create and manage cross-tenant API keys")
            );

            app.SetupRbkDefaultAdmin(options => options
                .WithUsername("superuser")
                .WithPassword("admin")
                .WithDisplayName("Administrator")
                .WithEmail("admin@my-company.com")
            );

            app.SeedDatabase<DatabaseSeed>();

            app.UseApplicationOptions();

            app.UseRbkUIDefinitions();

            // Register endpoints
            CreatePost.MapEndpoint(app);
            UpdatePost.MapEndpoint(app);
            GetSettings.MapEndpoint(app);
            DemoEndpoints.MapEndpoint(app);

            app.MapOpenApi().AllowAnonymous();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "Demo 1");
            });

            app.Run();
        }
    }
}
