using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using rbkApiModules.Utilities.Extensions;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Reflection;

namespace rbkApiModules.Infrastructure.Api
{
    public static class Builder
    {
        public static void AddRbkApiInfrastructureModule(this IServiceCollection services, Assembly[] assembliesForServices,
            Assembly[] assembliesForAutoMapper, List<IActionFilter> filters, string applicationName, string version, string swaggerXmlPath, bool isProduction)
        {
            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(Builder)));
            services.RegisterApplicationServices(assembliesForServices);

            services.AddHttpClient();

            services.AddMemoryCache();

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

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers(config => {
                foreach (var filter in filters)
                {
                    config.Filters.Add(filter);
                }
            });

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            if (isProduction)
            {
                // Para outros detalhes de configuração do HSTS e redirecionamento HTTPS
                // acessar https://aka.ms/aspnetcore-hsts
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(365);
                });
            }
        }

        public static IApplicationBuilder UseRbkApiDefaultSetup(this IApplicationBuilder app, bool isProduction)
        {
            if (isProduction)
            {
                // Para outros detalhes de configuração do HSTS e redirecionamento HTTPS
                // acessar https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            // Configuração do Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Default");
                
                c.RoutePrefix = "swagger";
                c.DocExpansion(DocExpansion.None);
            });

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Content-Disposition"));

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
                endpoints.MapFallbackToPage("/_Host");
            });

            return app;
        }
    }
}

