using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Analytics.Relational.SQLite;
using rbkApiModules.Analytics.Relational.SqlServer;
using rbkApiModules.Authentication;
using rbkApiModules.CodeGeneration;
using rbkApiModules.Comments;
using rbkApiModules.Demo.BusinessLogic;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.DataTransfer;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Demo.Services;
using rbkApiModules.Diagnostics.Core;
using rbkApiModules.Diagnostics.Relational.SQLite;
using rbkApiModules.Diagnostics.Relational.SqlServer;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Logs.Core;
using rbkApiModules.Logs.Relational.SQLite;
using rbkApiModules.Logs.Relational.SqlServer;
using rbkApiModules.Notifications;
using rbkApiModules.Payment.SqlServer;
using rbkApiModules.Paypal.SqlServer;
using rbkApiModules.SharedUI;
using rbkApiModules.UIAnnotations;
using rbkApiModules.Utilities.Excel;
using rbkApiModules.Workflow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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

        private readonly bool UseSQLite = false;

        private Assembly[] AssembliesForServices => new Assembly[]
        {
        };

        private Assembly[] AssembliesForAutoMapper => new Assembly[]
        {
            Assembly.GetAssembly(typeof(NotificationsMappings)),
            Assembly.GetAssembly(typeof(CommentsMappings)),
            Assembly.GetAssembly(typeof(RoleMappings)),
            Assembly.GetAssembly(typeof(PlanMappings)),
            Assembly.GetAssembly(typeof(UsersMapings)),
        }; 

        private Assembly[] AssembliesForMediatR => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CreateUser.Command)),
            Assembly.GetAssembly(typeof(CommentEntity.Command)),
            Assembly.GetAssembly(typeof(GenerateSpreadsheetTablesFromJson.Command)),
            Assembly.GetAssembly(typeof(RenewAccessToken.Command)),
            Assembly.GetAssembly(typeof(SharedUIController)),
            Assembly.GetAssembly(typeof(GetUiDefinitions.Command)),
            Assembly.GetAssembly(typeof(FilterAnalyticsEntries.Command)),
            Assembly.GetAssembly(typeof(FilterLogsEntries.Command)),
            Assembly.GetAssembly(typeof(FilterDiagnosticsEntries.Command)),
            Assembly.GetAssembly(typeof(CreateWebhookEvent.Command)),
            Assembly.GetAssembly(typeof(CreatePlan.Command)),
            Assembly.GetAssembly(typeof(DeleteNotification.Command)),
            // Assembly.GetAssembly(typeof(AuditingPostProcessingBehavior<,>))
        };

        private Assembly[] AssembliesUIDefinitions => new Assembly[]
        {
            Assembly.GetAssembly(typeof(Client)),
        };

        public void ConfigureServices(IServiceCollection services)
        {
            var connection = System.Environment.OSVersion.Platform == PlatformID.Unix ? "DockerConnection" : "DefaultConnection";

            services.AddDbContext<DbContext, DatabaseContext>((scope, options) => options
                .UseSqlServer(
                    Configuration.GetConnectionString(connection).Replace("**CONTEXT**", "Database"))
                .AddInterceptors(scope.GetRequiredService<DatabaseAnalyticsInterceptor>())
                .AddInterceptors(scope.GetRequiredService<DatabaseDiagnosticsInterceptor>())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
            );

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
                ForceHttps = true,
                AutomapperProfiles = new List<Profile>
                {
                    new StatesMappings<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>()
                }
            });

            services.AddRbkApiMediatRModule(AssembliesForMediatR);
            services.AddRbkApiMediatRModuleSqlServer();

            services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkSharedUIModule(Configuration);

            services.AddRbkApiCommentsModule();

            services.AddRbkApiNotificationsModule();

            services.AddRbkApiExcelModule();

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();

            services.AddRbkUIDefinitions(AssembliesUIDefinitions);

            // services.AddSqlServerRbkApiAnalyticsModule(Configuration, "Data Source=50.31.134.17;Integrated Security=False;Initial Catalog=VarejoFacil.Production_Analytics;User ID=sa;Password=zemiko98sql;Connect Timeout=15;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            // services.AddSqlServerRbkApiDiagnosticsModule("Data Source=50.31.134.17;Integrated Security=False;Initial Catalog=VarejoFacil.Production_Diagnostics;User ID=sa;Password=zemiko98sql;Connect Timeout=999;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            if (UseSQLite)
            {
                services.AddSQLiteRbkApiAnalyticsModule(Configuration, "Filename=analytics.db");
                services.AddSQLiteRbkApiDiagnosticsModule("Filename=diagnostics.db");
                services.AddSQLiteRbkApiLogsModule("Filename=logs.db");
            }
            else
            {
                services.AddSqlServerRbkApiAnalyticsModule(Configuration, Configuration.GetConnectionString(connection).Replace("**CONTEXT**", "Analytics"));
                services.AddSqlServerRbkApiDiagnosticsModule(Configuration.GetConnectionString(connection).Replace("**CONTEXT**", "Diagnostics"));
                services.AddSqlServerRbkApiLogsModule(Configuration.GetConnectionString(connection).Replace("**CONTEXT**", "Logs"));
            }    

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

            if (UseSQLite)
            {
                app.UseSQLiteRbkApiAnalyticsModule(options => options
                    .LimitToPath("/api")
                    .ExcludeMethods("OPTIONS")
                    .ExcludePath(new[] { "/api/test/download" })
                    .ExcludePath(new[] { "/analytics" })
                    .ExcludePath(new[] { "/diagnostics" })
                    .ExcludeStatusCodes(new[] { 401 })
                    .UseSessionAnalytics(5)
                    .UseDemoData()
                );

                app.UseSQLiteRbkApiDiagnosticsModule();

                app.UseSQLiteRbkApiLogsModule();
            }
            else
            {

                app.UseSqlServerRbkApiAnalyticsModule(options => options
                    .LimitToPath("/api")
                    .ExcludeMethods("OPTIONS")
                    .ExcludePath(new[] { "/api/test/download" })
                    .ExcludePath(new[] { "/analytics" })
                    .ExcludePath(new[] { "/diagnostics" })
                    .ExcludeStatusCodes(new[] { 401 })
                    .UseSessionAnalytics(5)
                    // .UseDemoData()
                );

                app.UseSqlServerRbkApiDiagnosticsModule();

                app.UseSqlServerRbkApiLogsModule();
            }


            app.UseRbkApiDefaultSetup(options => options
                .SetEnvironment(!Environment.IsDevelopment())
                // Configuration example
                .UseSpaOnRoot()
                .AddRoute("/patient", "/patient/index.html")
                .AddRoute("/professional", "/professional/index.html")
            );

            app.UseRbkApiAuthenticationModule(options => options
                .SeedAuthenticationClaims()
                .UseDefaultClaimDescriptions()
                .AddAuthenticationGroup("manager")
                .AddAuthenticationGroup("client"));

            app.UseRbkSharedUIModule(options => options
                .UseAnalytics()
                .UseDiagnostics()
                .AddCustomRoute("/swagger/index.html", "Swagger", "fas fa-code"));

            app.UseRbkCodeGenerationModule(options => options
                .ForScope("project-a")
                    .IgnoreRoutesContaining("* api/access")
                    .IgnoreRoutesContaining("* api/comments")
                    .IgnoreRoutesContaining("* api/diagnostics")
                    .IgnoreRoutesContaining("* api/notifications")
                    .IgnoreRoutesContaining("* api/paypal")
                    .IgnoreRoutesContaining("* api/plans")
                    .IgnoreRoutesContaining("* api/subscriptions")
                    .IgnoreRoutesContaining("* api/trial-keys")
                    .IgnoreRoutesContaining("* api/auth")
                .ForScope("project-b")
                    .IgnoreRoutesContaining("* api/access")
                    .IgnoreRoutesContaining("* api/comments")
                    .IgnoreRoutesContaining("* api/diagnostics")
                    .IgnoreRoutesContaining("* api/notifications")
                    .IgnoreRoutesContaining("* api/paypal")
                    .IgnoreRoutesContaining("* api/plans")
                    .IgnoreRoutesContaining("* api/subscriptions")
                    .IgnoreRoutesContaining("* api/trial-keys")
                    .IgnoreRoutesContaining("* api/auth"));
        }
    }
}
