using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using System.Reflection;
using System.Text.Json;
using rbkApiModules.Commons.Core.UiDefinitions;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Pipelines;
using Microsoft.EntityFrameworkCore;

namespace Demo6.Proxy;

public class Startup
{
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;
    private readonly bool _isInTestMode;

    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        _configuration = configuration;
        _environment = environment;

        _isInTestMode = AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName.ToLowerInvariant().StartsWith("tunit"));

        var temp = environment.EnvironmentName;
    }

    public IConfiguration Configuration => _configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        //services.AddDbContext<RelationalContext>((scope, options) => options
        //    .UseNpgsql(
        //        _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database.Relational"))
        ////.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
        ////.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
        ////.EnableDetailedErrors()
        ////.EnableSensitiveDataLogging()
        //);

        //services.AddScoped<DbContext, RelationalContext>();

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
            // .UseDefaultHsts(_environment.IsDevelopment())
            // .UseDefaultHttpsRedirection()
            .UseDefaultMemoryCache()
            .UseDefaultPipelines()
            .SuppressPipeline(typeof(TransactionBehavior<,>))
            .SuppressPipeline(typeof(CqrsReplicaBehavior<,>))
            .UseDefaultSwagger("Multi API Integration Tests")
            .UseDefaultGlobalErrorHandler()
            .UseHttpContextAccessor()
            .UseStaticFiles()
        );

        services.AddHttpClient("ProcessingApi", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5125");
        });

        services.AddRbkUIDefinitions(AssembliesForUiDefinitions);
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRbkApiCoreSetup();

        app.UseMiddleware<RequestContextLoggerMiddleware>();
    }

    private static Assembly[] AssembliesForAutoMapper => Array.Empty<Assembly>();

    private static Assembly[] AssembliesForMediatR =>
    [
        Assembly.GetAssembly(typeof(Startup))
    ];

    private static Assembly[] AssembliesForAdditionalValidations =>
    [
        Assembly.GetAssembly(typeof(Startup))
    ];

    private static Assembly[] AssembliesForUiDefinitions => Array.Empty<Assembly>();

    private static Assembly[] AssembliesForServices =>
    [
        Assembly.GetAssembly(typeof(Startup))
    ];
}

