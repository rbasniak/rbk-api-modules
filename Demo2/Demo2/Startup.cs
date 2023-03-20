using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Text.Json;
using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Pipelines;
using Demo2.Samples.Relational.Database;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Database;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Commands.ChangeRequests;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Projectors;

namespace Demo2.Api;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly bool _isInTestMode;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;

        _isInTestMode = AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("xunit"));

        var temp = environment.EnvironmentName;
    }

    public IConfiguration Configuration => _configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddDbContext<ReadDatabaseContext>((scope, options) => options
        //    .UseSqlServer(
        //        _configuration.GetConnectionString(readConnection).Replace("**CONTEXT**", "Database.Read"))
        //    //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
        //    //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
        //    .EnableDetailedErrors()
        //    .EnableSensitiveDataLogging()
        //);

        services.AddDbContext<EventSourcingContext>((scope, options) => options
            .UseNpgsql(
                _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database.EventSourcing"))
            //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
            //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
            //.EnableDetailedErrors()
            //.EnableSensitiveDataLogging()
        );

        services.AddDbContext<RelationalContext>((scope, options) => options
            .UseNpgsql(
                _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database.Relational"))
            //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
            //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
            //.EnableDetailedErrors()
            //.EnableSensitiveDataLogging()
        );

        services.AddScoped<DbContext, EventSourcingContext>();
        services.AddScoped<DbContext, RelationalContext>();

        services.AddRbkApiRelationalSetup(options => options
            .EnableBasicAuthenticationHandler()
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
            .UseDefaultHttpsRedirection(_isInTestMode)
            .UseDefaultMemoryCache()
            .UseDefaultPipelines()
            .SuppressPipeline(typeof(TransactionBehavior<,>))
            .SuppressPipeline(typeof(CqrsReplicaBehavior<,>))
            .UseDefaultSwagger("PoC for the new API libraries")
            .UseHttpContextAccessor()
            .UseStaticFiles()
        );

        services.AddSingleton(new ListOfValuesRepository());

        services.AddTransient<IEventStore, RelationalEventStore>();

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

        app.UseMiddleware<RequestContextLoggerMiddleware>();

        app.UseRbkApiCoreSetup();

        app.SetupDatabase<EventSourcingContext>(options => options
            .MigrateOnStartup()
            .ResetOnStartup(_isInTestMode)
        );

        app.SetupDatabase<RelationalContext>(options => options
            .MigrateOnStartup()
            .ResetOnStartup(_isInTestMode)
        );

        //app.SetupDatabase<ReadDatabaseContext>(options => options
        //    .MigrateOnStartup()
        //    .ResetOnStartup()
        //);

        //app.SetupRbkAuthenticationClaims(options => options
        //    .WithCustomDescription(x => x.ChangeClaimProtection, "Change claim protection")
        //    .WithCustomDescription(x => x.ManageClaims, "Manage application claims")
        //    .WithCustomDescription(x => x.ManageTenantSpecificRoles, "Manage tenant roles")
        //    .WithCustomDescription(x => x.ManageApplicationWideRoles, "Manage application roles")
        //    .WithCustomDescription(x => x.ManageTenants, "Manage tenants")
        //    .WithCustomDescription(x => x.ManageUsers, "Manage users")
        //    .WithCustomDescription(x => x.ManageUserRoles, "Manage user roles")
        //    .WithCustomDescription(x => x.OverrideUserClaims, "Override user claims")
        //);

        //app.SetupRbkDefaultAdmin(options => options
        //    .WithUsername("superuser")
        //    .WithPassword("admin")
        //    .WithDisplayName("Administrator")
        //    .WithEmail("admin@my-company.com")
        //);

        //app.SeedDatabase<DatabaseSeed>();
    }

    private static Assembly[] AssembliesForAutoMapper => new Assembly[0];

    private static Assembly[] AssembliesForMediatR => new[] 
    {
        Assembly.GetAssembly(typeof(CreateChangeRequestByGeneralUser.Request))
    };

    private static Assembly[] AssembliesForAdditionalValidations => new[]
    {
        Assembly.GetAssembly(typeof(CreateChangeRequestByGeneralUser.Validator))
    };

    private static Assembly[] AssembliesForUiDefinitions => new Assembly[0];

    private static Assembly[] AssembliesForServices => new[]
    {
        Assembly.GetAssembly(typeof(IEventStore))
    };

    // TODO: Add a simple example filter 
    private static IActionFilter[] MvcFilters => new IActionFilter[0];
}

