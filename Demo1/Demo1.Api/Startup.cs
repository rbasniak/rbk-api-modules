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
using AutoMapper;
using rbkApiModules.Faqs.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Core.CQRS;
using rbkApiModules.Comments.Relational;
using rbkApiModules.Comments.Core;
using rbkApiModules.Commons.Core;
using rbkApiModules.Notifications.Relational;
using Demo1.BusinessLogic.Queries;
using rbkApiModules.Commons.Relational.CQRS;
using Serilog;
using rbkApiModules.Commons.Core.Pipelines;
using rbkApiModules.Identity.Core.DataTransfer.Tenants;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Cors.Infrastructure;

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
        var writeConnection = Environment.OSVersion.Platform == PlatformID.Unix ? "DockerWriteConnection" : "DefaultWriteConnection";
        var readConnection = Environment.OSVersion.Platform == PlatformID.Unix ? "DockerReadConnection" : "DefaultReadConnection";

        var temp1 = _configuration.GetConnectionString(writeConnection);
        var temp2 = _configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ReadDatabaseContext>((scope, options) => options
                .UseNpgsql(
                    _configuration.GetConnectionString(readConnection).Replace("**CONTEXT**", "Database.Read"))
                //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

            services.AddDbContext<DatabaseContext>((scope, options) => options
                .UseNpgsql(
                    _configuration.GetConnectionString(writeConnection).Replace("**CONTEXT**", "Database.Write"))
                //.AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                //.AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

        services.AddScoped<DbContext, DatabaseContext>();
        services.AddScoped<DbContext, ReadDatabaseContext>();

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
        app.UseSerilogRequestLogging();

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

