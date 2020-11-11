using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Comments;
using rbkApiModules.Demo.Database;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Services;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.UIAnnotations;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Demo.BusinessLogic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Filters;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Analytics.SqlServer;
using rbkApiModules.Analytics.UI;
using rbkApiModules.Auditing.UI;
using rbkApiModules.SharedUI;
using rbkApiModules.Diagnostics.SqlServer;
using rbkApiModules.Diagnostics.UI;
using rbkApiModules.Diagnostics.Core;
using rbkApiModules.Demo.Models;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        private Assembly[] AssembliesForServices => new Assembly[]
        {
            Assembly.GetAssembly(typeof(AnalyticsDataService)),
            Assembly.GetAssembly(typeof(DiagnosticsDataService))
        };

        private Assembly[] AssembliesForAutoMapper => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CommentsMappings)),
            Assembly.GetAssembly(typeof(UserMappings)),
        };

        private Assembly[] AssembliesBlazorRouting => new Assembly[]
        {
            Assembly.GetAssembly(typeof(IAnalyticsDataService)),
            Assembly.GetAssembly(typeof(IDiagnosticsDataService)),
            Assembly.GetAssembly(typeof(Class2)),
        };

        private Assembly[] AssembliesForMediatR => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CreateUser.Command)),
            Assembly.GetAssembly(typeof(CommentEntity.Command)),
            Assembly.GetAssembly(typeof(UserLogin.Command)),
            Assembly.GetAssembly(typeof(GetUiDefinitions.Command)),
            Assembly.GetAssembly(typeof(FilterAnalyticsEntries.Command)),
            Assembly.GetAssembly(typeof(FilterDiagnosticsEntries.Command)),
            // Assembly.GetAssembly(typeof(AuditingPostProcessingBehavior<,>))
        };

        private Assembly[] AssembliesUIDefinitions => new Assembly[]
        {
            Assembly.GetAssembly(typeof(Client)),
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>((scope, options) => options
                .UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database"))
                .AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                .AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

            services.AddTransient<DbContext, DatabaseContext>();

            services.AddRbkSharedUIModule(AssembliesBlazorRouting, new RbkSharedUIModuleOptions 
            {
                UseAnalyticsRoutes = true,
                UseAuditingRoutes = true,
                UseDiagnosticsRoutes = true,
                CustomRoutes = new List<RouteDefinition>
                {
                    new RouteDefinition("/swagger/index.html", "Swagger")
                }
            });

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            services.AddRbkApiInfrastructureModule(AssembliesForServices, AssembliesForAutoMapper,
                new List<IActionFilter> { new AnalyticsMvcFilter() },
                "RbkApiModules Demo API", "v1", xmlPath, !Environment.IsDevelopment());

            services.AddRbkApiMediatRModule(AssembliesForMediatR);

            services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkApiCommentsModule();

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();

            services.AddRbkUIDefinitions(AssembliesUIDefinitions);

            services.AddSqlServerRbkApiAnalyticsModule(Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics"));

            services.AddSqlServerRbkApiDiagnosticsModule(Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Diagnostics"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSqlServerRbkApiAnalyticsModule(options => options
                .LimitToPath("/api")
                .ExcludeMethods("OPTIONS")
                .UseDemoData()
            );

            app.UseSqlServerRbkApiDiagnosticsModule();

            app.UseRbkApiDefaultSetup(!Environment.IsDevelopment());
        }
    }
}
