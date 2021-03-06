﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication;
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
using System.Diagnostics.CodeAnalysis;
using System.IO.Compression;
using System.Reflection;

namespace rbkApiModules.Infrastructure.Api
{
    public class RbkApiInfrastructureModuleOptions
    {
        public Assembly[] AssembliesForServices { get; set; }
        public Assembly[] AssembliesForAutoMapper { get; set; }
        public List<IActionFilter> Filters { get; set; }
        public string ApplicationName { get; set; }
        public string Version { get; set; }
        public string SwaggerXmlPath { get; set; }
        public bool IsProduction { get; set; }
        public bool UseBasicAuthentication { get; set; }
        public List<Profile> AutomapperProfiles { get; set; }
    }

    [ExcludeFromCodeCoverage]
    public static class Builder
    {
        public static void AddRbkApiInfrastructureModule(this IServiceCollection services, RbkApiInfrastructureModuleOptions options)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);

            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(Builder)));
            
            if (options.AssembliesForServices != null)
            {
                services.RegisterApplicationServices(options.AssembliesForServices);
            }

            services.AddHttpClient();

            services.AddMemoryCache();

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var config = new MapperConfiguration(cfg =>
            {
                if (options.AssembliesForAutoMapper != null)
                {
                    cfg.ApplyProfiles(options.AssembliesForAutoMapper);
                }

                cfg.CreateMap<Guid?, string>().ConvertUsing(g => g.HasValue ? g.Value.ToString().ToLower() : null);
                cfg.CreateMap<Guid, string>().ConvertUsing(g => g.ToString().ToLower());

                if (options.AutomapperProfiles != null)
                {
                    foreach (var profile in options.AutomapperProfiles)
                    {
                        cfg.AddProfile(profile);
                    }
                }
            });
            var mapper = config.CreateMapper();
            services.AddSingleton(mapper);
            mapper.ConfigurationProvider.AssertConfigurationIsValid();

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });

            if (options.Filters != null)
            {
                services.AddControllers(config =>
                {
                    foreach (var filter in options.Filters)
                    {
                        config.Filters.Add(filter);
                    }
                });
            }

            services.AddRazorPages();
            services.AddServerSideBlazor();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddCors();

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = options.ApplicationName, Version = options.Version });

                config.IncludeXmlComments(options.SwaggerXmlPath);

                config.CustomSchemaIds(x => x.FullName);

                config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Description = "Please insert JWT with Bearer into field",
                });

                config.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            services.AddAuthentication(BasicAuthenticationHandler.Basic)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.Basic, null);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            if (options.IsProduction)
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

        public static IApplicationBuilder UseRbkApiDefaultSetup(this IApplicationBuilder app, bool isProduction, bool useSharedUI = true)
        {
            if (isProduction)
            {
                // Para outros detalhes de configuração do HSTS e redirecionamento HTTPS
                // acessar https://aka.ms/aspnetcore-hsts
                app.UseHsts();
            }

            app.UseResponseCompression();

            app.UseHttpsRedirection();

            app.UseSwagger();

            // Configuração do Swagger
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "Default");
                
                c.RoutePrefix = "swagger";
                c.DocExpansion(DocExpansion.None);
            });

            app.UseCors(c => c.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().WithExposedHeaders("Content-Disposition"));

            app.UseAuthentication();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                // endpoints.MapRazorPages();
                if (useSharedUI)
                {
                    endpoints.MapFallbackToPage("/_Host");
                }
            });

            return app;
        }
    }
}

