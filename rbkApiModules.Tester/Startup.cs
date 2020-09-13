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
using rbkApiModules.Analytics;

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
        };

        private Assembly[] AssembliesForAutoMapper => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CommentsMappings)),
            Assembly.GetAssembly(typeof(UserMappings)),
        };

        private Assembly[] AssembliesForMediatR => new Assembly[]
        {
            Assembly.GetAssembly(typeof(CommentEntity.Command)),
            Assembly.GetAssembly(typeof(UserLogin.Command)),
            Assembly.GetAssembly(typeof(GetUiDefinitions.Command)),
            Assembly.GetAssembly(typeof(AuditingPostProcessingBehavior<,>))
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database")));

            services.AddDbContext<AuditingContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Auditing")));

            //// **************************************************************************
            //// *** Descomentar só para criar novas migrations no projeto de analytics ***
            //// **************************************************************************
            //services.AddDbContext<SqlServerContext>(options =>
            //   options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics")));

            services.AddTransient<DbContext, DatabaseContext>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            services.AddRbkApiInfrastructureModule(AssembliesForServices, AssembliesForAutoMapper, "RbkApiModules Demo API", "v1", 
                xmlPath, !Environment.IsDevelopment());

            services.AddRbkApiMediatRModule(AssembliesForMediatR);

            services.AddRbkApiAuthenticationModule(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkApiCommentsModule();

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseServerSideAnalytics(new SqlServerAnalyticStore(
                Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics")))
                    .LimitToPath("/api")
                    .ExcludeMethod("OPTIONS");

            app.UseRbkApiDefaultSetup(!Environment.IsDevelopment());
        }
    }
}
