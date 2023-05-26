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

namespace Demo3
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

            if (TestingEnvironmentChecker.IsTestingEnvironment && TestingMode.GetMode("Demo3") == DatabaseType.SQLite || providerForMigration?.ToLower() == "sqlite")
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

            //services.AddDbContext<DatabaseContext>((scope, options) => options
            //    .UseNpgsql(
            //        _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Application"))
            //    .EnableDetailedErrors()
            //    .EnableSensitiveDataLogging()
            //);

            //services.AddScoped<DbContext, DatabaseContext>();

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
                //.UseCustomCors("_proteusPolicy", options =>
                //        options.AddPolicy(name: "_proteusPolicy",
                //                      builder =>
                //                      {
                //                          builder.AllowAnyMethod()
                //                          .AllowAnyHeader()
                //                          .WithOrigins("http://localhost:4107")
                //                          .AllowCredentials()
                //                          .WithExposedHeaders("Content-Disposition");
                //                      }))
                .UseDefaultHsts(_environment.IsDevelopment())
                .UseDefaultHttpsRedirection()
                .UseDefaultMemoryCache()
                .UseDefaultHttpClient()
                .UseDefaultPipelines()
                .UseDefaultHttpClient()
                .UseDefaultSwagger("Demo using Windows authentication")
                .UseHttpContextAccessor()
                .UseStaticFiles()
                .UseDefaultLocalizationLanguage("pt-br")
                .SuppressPipeline(typeof(CqrsReplicaBehavior<,>))
            );

            services.AddRbkRelationalAuthentication(options => options
                .UseSymetricEncryptationKey()
                .UseLoginWithWindowsAuthentication()
                .AllowAnonymousAccessToTenants()
                .AllowUserCreationByAdmins()
                .AllowTenantSwitching()
            // .UseMockedWindowsAuthentication()
            );

            services.AddRbkUIDefinitions(AssembliesForUiDefinitions);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler(builder =>
            {
                builder.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

                    var errorHandler = context.Features.Get<IExceptionHandlerFeature>();
                    if (errorHandler != null)
                    {
                        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

                        // Must create a new scope because if we have any errors while saving the diagnostics data, the
                        // invalid data will be kept in the context and EF will tries to save it again
                        using (var scope = scopeFactory.CreateScope())
                        {
                            var logger = scope.ServiceProvider.GetService<Serilog.ILogger>();

                            logger.Fatal(errorHandler.Error, "Exception caught by the global exception handler");
                        }

                        await context.Response.WriteAsync(
                            JsonSerializer.Serialize(new { Errors = new string[] { "Server internal error." } }))
                                .ConfigureAwait(false);
                    }
                });
            });

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