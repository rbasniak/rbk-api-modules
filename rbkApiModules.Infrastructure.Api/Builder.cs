using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using rbkApiModules.Utilities.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;

namespace rbkApiModules.Infrastructure.Api
{
    public static class BuilderExtensions
    {
        public static void AddRbkApiInfrastructureModule(this IServiceCollection services, Assembly[] assembliesForServices,
            Assembly[] assembliesForAutoMapper, string applicationName, string version, string swaggerXmlPath)
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(BuilderExtensions)));
            services.RegisterApplicationServices(assembliesForServices);

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.ApplyProfiles(assembliesForAutoMapper);
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = applicationName, Version = version });

                options.IncludeXmlComments(swaggerXmlPath);

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

            services.AddHttpClient();

            services.AddMemoryCache();

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        public static IApplicationBuilder UseRbkApiDefaultSetup(this IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseSwagger();

            // Configuração do Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default");
                
                c.RoutePrefix = "swagger";
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
                endpoints.MapBlazorHub();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });

            return app;
        }
    }
}

