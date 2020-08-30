using System;
using System.IO;
using System.Reflection;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using rbkApiModules.Comments;
using rbkApiModules.Tester.Database;
using Swashbuckle.AspNetCore.SwaggerUI;
using rbkApiModules.Infrastructure;
using rbkApiModules.Authentication;
using AspNetCoreApiTemplate.Auditing;
using rbkApiModules.Auditing;
using rbkApiModules.Analytics;
using rbkApiModules.Tester.Services;

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
        };

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Database")));

            services.AddDbContext<AuditingContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Auditing")));

            // **************************************************************************
            // *** Descomentar só para criar novas migrations no projeto de analytics ***
            // **************************************************************************
            services.AddDbContext<SqlServerContext>(options =>
               options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics")));

            services.RegisterApplicationServices(AssembliesForServices);

            services.AddTransient<DbContext, DatabaseContext>();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddRbkApiModulesInfrastructure();

            services.AddRbkApiModulesAuthentication(Configuration.GetSection(nameof(JwtIssuerOptions)));

            services.AddRbkApiModulesComments();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.ApplyProfiles(new[] {
                    Assembly.GetAssembly(typeof(CommentsMappings)),
                    Assembly.GetAssembly(typeof(UserMappings)),
                });
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "RbkApiModules Demo API", Version = "v1" });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                options.CustomSchemaIds(x => x.FullName);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Please insert JWT with Bearer into field",
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            services.AddScoped<IUserdataCommentService, UserdataCommentService>();

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(FailFastRequestBehavior<,>));
            services.AddMediatR(
                Assembly.GetAssembly(typeof(CommentEntity.Command)),
                Assembly.GetAssembly(typeof(UserLogin.Command)),
                Assembly.GetAssembly(typeof(AuditingPostProcessingBehavior<,>)));

            // Configuração para desabilitar a validação default da API Core para os controllers
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseServerSideAnalytics(new SqlServerAnalyticStore(
                Configuration.GetConnectionString("DefaultConnection").Replace("**CONTEXT**", "Analytics")))
                    .LimitToPath("/api")
                    .ExcludeMethod("OPTIONS");

            app.UseSwagger();

            // Configuração do Swagger
            app.UseSwaggerUI(c =>
            {
                if (env.IsDevelopment())
                {
                    // For Debug in Kestrel
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "AspNetCore API Template");
                }
                else
                {
                    // To deploy on IIS
                    c.SwaggerEndpoint("/AspNetCoreApiTemplate/swagger/v1/swagger.json", "AspNetCore API Template");
                }

                c.RoutePrefix = string.Empty;
                c.DocExpansion(DocExpansion.None);
            });

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Content-Disposition"));

            app.UseHttpsRedirection();
            app.UseAntiXssMiddleware();
            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
