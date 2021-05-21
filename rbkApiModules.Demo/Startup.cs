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
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using AutoMapper;
using rbkApiModules.Workflow;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.Paypal.SqlServer;

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
            Assembly.GetAssembly(typeof(RoleMappings)),
            Assembly.GetAssembly(typeof(PlanMappings)),
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
            Assembly.GetAssembly(typeof(CreateWebhookEvent.Command)),
            Assembly.GetAssembly(typeof(CreatePlan.Command)),
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
            services.AddRbkApiInfrastructureModule(new RbkApiInfrastructureModuleOptions
            {
                AssembliesForServices = AssembliesForServices,
                AssembliesForAutoMapper = AssembliesForAutoMapper,
                Filters = new List<IActionFilter> { new AnalyticsMvcFilter() },
                ApplicationName = "RbkApiModules Demo API",
                Version = "v1",
                SwaggerXmlPath = xmlPath,
                IsProduction = !Environment.IsDevelopment(),
                AutomapperProfiles = new List<Profile>
                {
                    new StatesMappings<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>()
                }
            });

            services.AddRbkApiMediatRModule(AssembliesForMediatR);
            services.AddRbkApiMediatRModuleSqlServer();

            services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkApiCommentsModule();

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();

            services.AddRbkUIDefinitions(AssembliesUIDefinitions);

            services.AddSqlServerRbkApiAnalyticsModule(Configuration, Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics"));

            services.AddSqlServerRbkApiDiagnosticsModule(Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Diagnostics"));

            services.AddRbkApiPaypalModule<PaypalActions>();

            services.AddRbkApiPaymentModule<SubscriptionActions, TrialKeyActions>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            //app.Use(async (context, next) => {
            //    context.Request.EnableBuffering();
            //    await next();
            //});

            app.UseSqlServerRbkApiAnalyticsModule(options => options
                .LimitToPath("/api")
                .ExcludeMethods("OPTIONS")
                .ExcludePath(new[] { "/api/test/download" })
                // .UseDemoData()
            );

            app.UseSqlServerRbkApiDiagnosticsModule();

            app.UseRbkApiDefaultSetup(options => options
                .SetEnvironment(!Environment.IsDevelopment())
                // Configuration example
                //.AddRoute(new ApplicationRoute("/patient", "/patient/index.html"))
                //.AddRoute(new ApplicationRoute("/professional", "/professional/index.html"))
            );

            app.UseRbkApiAuthenticationModule(options => options
                .SeedAuthenticationClaims()
                .UseDefaultClaimDescriptions()
                .AddAuthenticationGroup("manager")
                .AddAuthenticationGroup("client"));
        }
    }
}
