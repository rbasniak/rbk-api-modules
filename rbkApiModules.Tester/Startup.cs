using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using rbkApiModules.Comments;
using rbkApiModules.Tester.Database;
using rbkApiModules.Authentication;
using rbkApiModules.Auditing;
using rbkApiModules.Tester.Services;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.Api;
using rbkApiModules.UIAnnotations;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Auditing.SqlServer;
using rbkApiModules.Tester.BusinessLogic;
using System.Collections.Generic;
using rbkApiModules.Logging.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using rbkApiModules.Analytics.Core;
using rbkApiModules.Analytics.SqlServer;
using rbkApiModules.Auditing.Core;
using rbkApiModules.Analytics.UI;

namespace rbkApiModules.Tester
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
            Assembly.GetAssembly(typeof(AnalyticsDataService))
        };

        private Assembly[] AssembliesForAutoMapper => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CommentsMappings)),
            Assembly.GetAssembly(typeof(UserMappings)),
        };

        private Assembly[] AssembliesForMediatR => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CreateUser.Command)),
            Assembly.GetAssembly(typeof(CommentEntity.Command)),
            Assembly.GetAssembly(typeof(UserLogin.Command)),
            Assembly.GetAssembly(typeof(GetUiDefinitions.Command)),
            Assembly.GetAssembly(typeof(FilterAnalyticsEntries.Command)),
            // Assembly.GetAssembly(typeof(AuditingPostProcessingBehavior<,>))
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database"))
                .AddInterceptors(new DatabaseLogIntgerceptor())
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging());

            //services.AddDbContext<AuditingContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Auditing"))); 

            services.AddTransient<DbContext, DatabaseContext>();

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            services.AddRbkApiInfrastructureModule(AssembliesForServices, AssembliesForAutoMapper, 
                new List<IActionFilter> { new AnalyticsMvcFilter() },
                "RbkApiModules Demo API", "v1", xmlPath, !Environment.IsDevelopment());

            services.AddRbkApiMediatRModule(AssembliesForMediatR);

            services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkApiCommentsModule();

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();

            services.AddSqlServerRbkApiAnalyticsModule(Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics"));

            // services.AddSqlServerRbkApiAuditingModule(Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Auditing"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseSqlServerRbkApiAnalyticsModule()
                .LimitToPath("/api")
                .ExcludeMethods("OPTIONS");

            // app.UseSqlServerRbkApiAuditingModule();

            app.UseRbkApiDefaultSetup(!Environment.IsDevelopment());
        }
    }
}
