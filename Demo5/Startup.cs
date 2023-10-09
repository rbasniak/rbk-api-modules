using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Text.Json;
using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Pipelines;
using rbkApiModules.Identity.Core.DataTransfer.Tenants;
using rbkApiModules.Commons.Core.Utilities;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Demo5
{

    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public IConfiguration Configuration => _configuration;

        public void ConfigureServices(IServiceCollection services)
        {
            var providerForMigration = _configuration.GetValue<string>("provider");

            if (TestingEnvironmentChecker.IsTestingEnvironment && TestingMode.GetMode("Demo5") == DatabaseType.SQLite || providerForMigration?.ToLower() == "sqlite")
            {
                services.AddDbContext<TestingDatabaseContext>((scope, options) => options
                    .UseSqlite($@"Data Source=integration_test.db")
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                );

                services.AddScoped<DbContext, TestingDatabaseContext>();
                services.AddScoped<DatabaseContext, TestingDatabaseContext>();
            }
            else
            {
                services.AddDbContext<DatabaseContext>((scope, options) => options
                    .UseNpgsql(
                        _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Application"))
                    .EnableDetailedErrors()
                    .EnableSensitiveDataLogging()
                );

                services.AddScoped<DbContext, DatabaseContext>();
            }

            services.AddRbkApiRelationalSetup(options => options
                .RegisterMediatR(AssembliesForMediatR)
                .RegisterServices(AssembliesForServices)
                .RegisterMappings(AssembliesForAutoMapper)
                .RegisterAdditionalValidators(AssembliesForAdditionalValidations)
                .SuppressModelStateInvalidFilter()
                .UseDefaulFluentValidationGlobalBehavior()
                .UseDefaultApiControllers()
                .UseDefaultApiRouting()
                .UseDefaultAutoMapper()
                .UseDefaultCompression()
                .UseDefaultCors()
                .UseDefaultHsts(_environment.IsDevelopment())
                .UseDefaultHttpsRedirection()
                .UseDefaultMemoryCache()
                .UseDefaultHttpClient()
                .UseDefaultPipelines()
                .UseDefaultHttpClient()
                .UseDefaultSwagger("Demo using two different authentication schemes")
                .UseHttpContextAccessor()
                .UseStaticFiles()
                .UseDefaultLocalizationLanguage("en-us")
                .SuppressPipeline(typeof(CqrsReplicaBehavior<,>))
                .UseDefaultGlobalErrorHandler()
            );

            var rsa = RSA.Create();
            rsa.ImportFromPem(File.ReadAllText("test.pub.pem"));

            services.AddRbkRelationalAuthentication(options => options
                .UseSymetricEncryptationKey()
                .AppendAuthenticationSchemes(new Action<AuthenticationBuilder>[]
                {
                    builder => builder.AddJwtBearer("Legacy", options => {

                        options.IncludeErrorDetails = true; 
                                                            
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            IssuerSigningKey = new RsaSecurityKey(rsa),
                            ValidAudience = "LEGACY APPLICATION",
                            ValidIssuer = "LEGACY AUTHENTICATION",
                            RequireSignedTokens = true,
                            RequireExpirationTime = true, 
                            ValidateLifetime = true, 
                            ValidateAudience = true,
                            ValidateIssuer = true,
                        };
                    })
                })
            );

            services.AddRbkUIDefinitions(AssembliesForUiDefinitions);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRbkApiCoreSetup();

            app.SetupDatabase<DatabaseContext>(options => options
                .MigrateOnStartup()
            );

            app.SetupRbkAuthenticationClaims();

            app.SetupRbkDefaultAdmin(options => options
                .WithUsername("superuser")
                .WithPassword("admin")
                .WithDisplayName("Administrator")
                .WithEmail("admin@my-company.com")
            );

            app.SeedDatabase<DatabaseSeed>();
        }

        private static Assembly[] AssembliesForAutoMapper => new[]
        {
        Assembly.GetAssembly(typeof(TenantMappings)),
        Assembly.GetAssembly(typeof(Startup)),
    };

        private static Assembly[] AssembliesForMediatR => new[]
        {
        Assembly.GetAssembly(typeof(Startup)),
        Assembly.GetAssembly(typeof(UserLogin)),
        Assembly.GetAssembly(typeof(UiDefinitionsController)),
    };

        private static Assembly[] AssembliesForAdditionalValidations => new[]
        {
        Assembly.GetAssembly(typeof(Startup))
    };

        private static Assembly[] AssembliesForUiDefinitions => new[]
        {
        Assembly.GetAssembly(typeof(Startup))
    };

        private static Assembly[] AssembliesForServices => new[]
        {
        Assembly.GetAssembly(typeof(Startup))
    };

        private static IActionFilter[] MvcFilters => new IActionFilter[0];
    }

}