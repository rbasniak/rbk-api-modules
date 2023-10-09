using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Reflection;
using System.Text.Json;
using Demo1.BusinessLogic.Commands;
using Demo1.Database.Domain;
using Demo1.Database.Read;
using Demo1.DataTransfer;
using rbkApiModules.Faqs.Relational;
using rbkApiModules.Commons.Core.UiDefinitions;
using Demo1.Models.Domain.Demo;
using rbkApiModules.Faqs.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Comments.Relational;
using rbkApiModules.Comments.Core;
using rbkApiModules.Commons.Core;
using rbkApiModules.Notifications.Relational;
using Demo1.BusinessLogic.Queries;
using Serilog;
using rbkApiModules.Commons.Core.Pipelines;
using rbkApiModules.Identity.Core.DataTransfer.Tenants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Commons.Core.Utilities;
using System;
using System.Diagnostics;

namespace Demo1.Api;

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

        if (TestingEnvironmentChecker.IsTestingEnvironment && TestingMode.GetMode("Demo1") == DatabaseType.SQLite || providerForMigration?.ToLower() == "sqlite")
        {
            throw new NotSupportedException("SQLite is not supported in this project");
        }
        else
        {
            services.AddDbContext<ReadDatabaseContext>((scope, options) => options
                .UseNpgsql(
                    _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Read"))
                //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

            services.AddDbContext<DatabaseContext>((scope, options) => options
                .UseNpgsql(
                    _configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Write"))
                //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

            services.AddScoped<DbContext, DatabaseContext>();
            services.AddScoped<DbContext, ReadDatabaseContext>();
        }

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
            .UseDefaultSwagger("PoC for the new API libraries")
            .UseDefaultSwagger("PoC for the new API libraries")
            .UseHttpContextAccessor()
            .UseStaticFiles()
            .SuppressPipeline(typeof(CqrsReplicaBehavior<,>))
            .UseDefaultGlobalErrorHandler()
            //.UseSimpleCqrs(options => options
            //    .ForType<Models.Read.Post, CqrsRelationalStore>()
            //    .ForType<Models.Read.Blog, CqrsInMemoryStore>((services) => {
            //        using (var scope = services.CreateScope())
            //        {
            //            var context = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            //            var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

            //            var blogs = context.Blogs.ToList();

            //            var data = mapper.Map<List<Models.Read.Blog>>(blogs); ;

            //            return data.Select(x => (BaseEntity)x).ToArray();
            //        }
            //    })
            // )
            // .UseCustomMvcFilters(new AnalyticsMvcFilter())
        );

        services.AddRbkRelationalAuthentication(options => options
            .UseSymetricEncryptationKey()
            .AllowUserCreationByAdmins()
            .AllowUserSelfRegistration()
        );

        services.AddRbkUIDefinitions(AssembliesForUiDefinitions);

        services.AddRbkRelationalComments(options =>
            options.SetCommentsUserdataService(typeof(UserdataCommentService))
        );

        services.AddRbkRelationalNotifications();

        services.AddRbkRelationalFaqs();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRbkApiCoreSetup();

        app.UseSerilogRequestLogging();

        app.UseMiddleware<RequestContextLoggerMiddleware>();


        app.SetupDatabase<DatabaseContext>(options => options
            .MigrateOnStartup()
        );

        app.SetupDatabase<ReadDatabaseContext>(options => options
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
        );

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
        Assembly.GetAssembly(typeof(Models.Read.Mappings.PostMappings)),
        Assembly.GetAssembly(typeof(BlogMappings)),
        Assembly.GetAssembly(typeof(TenantMappings)),
        Assembly.GetAssembly(typeof(FaqMappings)),
        Assembly.GetAssembly(typeof(CommentsMappings)),
    };

    private static Assembly[] AssembliesForMediatR => new[]
    {
        Assembly.GetAssembly(typeof(PipelineValidationTest)),
        Assembly.GetAssembly(typeof(UserLogin)),
        Assembly.GetAssembly(typeof(CommentEntity)),
        Assembly.GetAssembly(typeof(CreateFaq)),
        Assembly.GetAssembly(typeof(UiDefinitionsController)),
        Assembly.GetAssembly(typeof(GetAllAuthors))
    };

    private static Assembly[] AssembliesForAdditionalValidations => new[]
    {
        Assembly.GetAssembly(typeof(QuantityValidator))
    };

    private static Assembly[] AssembliesForUiDefinitions => new[]
    {
        Assembly.GetAssembly(typeof(Blog))
    };

    private static Assembly[] AssembliesForServices => new[]
    {
        Assembly.GetAssembly(typeof(IService1))
    };

    // TODO: Add a simple example filter
    private static IActionFilter[] MvcFilters => new IActionFilter[0];
}

