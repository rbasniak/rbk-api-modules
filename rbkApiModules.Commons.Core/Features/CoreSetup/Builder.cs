using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using rbkApiModules.Commons.Core;
using System.IO.Compression;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("rbkApiModules.Commons.Relational")]

namespace Microsoft.Extensions.DependencyInjection;




public static class CommonsCoreBuilder
{
    public static void AddRbkApiCoreSetup(this IServiceCollection services, Action<RbkApiCoreOptions> optionsConfig)
    {
        var options = new RbkApiCoreOptions();
        optionsConfig(options);

        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;

        services.AddSingleton(options);

        services.AddMessaging();

        #region Response Compression

        if (options._useDefaultCompression)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        }

        if (options._userCompressionOptions != null)
        {
            if (options._useDefaultCompression)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomCompression)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultCompression)}, choose either one or another");
            }

            services.AddResponseCompression(options._userCompressionOptions);

            if (options._userProviderCompressionOptions != null)
            {
                services.Configure<GzipCompressionProviderOptions>(options._userProviderCompressionOptions);
            }
            else
            {
                services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            }
        }

        #endregion

        #region Application services

        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(CommonsCoreBuilder)));

        foreach (var assembly in options._assembliesForServices)
        {
            services.RegisterApplicationServices(options._assembliesForServices.ToArray());
        }

        #endregion

        #region Basic HttpClient

        if (options._useDefaultHttpClient)
        {
            services.AddHttpClient();
        }

        #endregion

        #region Memory cache

        if (options._useDefaultMemoryCache)
        {
            services.AddMemoryCache();
        }

        if (options._userMemoryCacheOptions != null)
        {
            if (options._useDefaultMemoryCache)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomMemoryCache)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultMemoryCache)}, choose either one or another");
            }

            services.AddMemoryCache(options._userMemoryCacheOptions);
        }

        #endregion

        #region IHttpContextAccessor registration

        if (options._useDefaultHttpClient)
        {
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        #endregion

        #region HTTPS redirection

        if (options._useDefaultHttpsRedirection)
        {
            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
        }

        if (options._userHttpsRedirectionOptions != null)
        {
            if (options._useDefaultHttpsRedirection)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseDefaultHttpsRedirection)} cannot be used with {nameof(RbkApiCoreOptions.UseCustomHttpsRedirection)}, choose either one or another");
            }

            services.AddHttpsRedirection(options._userHttpsRedirectionOptions);
        }

        #endregion

        #region CORS

        if (options._useDefaultCors)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowAnyOrigin()
                        .WithExposedHeaders("Content-Disposition");
                    });
            });
        }

        if (options._userCorsOptions != null)
        {
            if (options._useDefaultCors)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomCors)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultCors)}, choose either one or another");
            }

            services.AddCors(options._userCorsOptions);
        }

        #endregion

        #region Swagger

        if (options._useDefaultSwaggerOptions)
        {
            // services.AddEndpointsApiExplorer();

            // services.AddOpenApi();
            //services.ConfigureSwaggerGen(config =>
            //{
            //    config.CustomSchemaIds(x => x.FullName.Replace("+", "."));

            //    config.SwaggerDoc("identity", new OpenApiInfo { Title = "Identity API", Version = "v1" });

            //    config.DocInclusionPredicate((docName, apiDesc) =>
            //    {
            //        var tags = apiDesc.ActionDescriptor?.EndpointMetadata?
            //                .OfType<TagsAttribute>()
            //                .SelectMany(x => x.Tags)
            //                .Where(x => !string.IsNullOrEmpty(x))
            //                .ToList();

            //        if (tags.Contains("Roles") || tags.Contains("Tenants") || tags.Contains("Claims") || tags.Contains("Authentication") || tags.Contains("Authorization"))
            //        {
            //            return docName == "identity";
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    });

            //    config.AddSecurityDefinition("Api-Key", new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.ApiKey,
            //        In = ParameterLocation.Header,
            //        Name = "Api-Key",
            //        Description = "Please inser the API key in the 'value' field",
            //    });

            //    config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            //    {
            //        Type = SecuritySchemeType.ApiKey,
            //        In = ParameterLocation.Header,
            //        Name = "Authorization",
            //        Scheme = "Bearer",
            //        BearerFormat = "JWT",
            //        Description = "Please insert the JWT token prefixed with 'Bearer' in the 'value' field",
            //    });

            //    config.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //          {
            //              new OpenApiSecurityScheme
            //              {
            //                  Reference = new OpenApiReference
            //                  {
            //                      Type = ReferenceType.SecurityScheme,
            //                      Id = "Api-Key"
            //                  }
            //              },
            //              Array.Empty<string>()
            //          },
            //          {
            //              new OpenApiSecurityScheme
            //              {
            //                  Reference = new OpenApiReference
            //                  {
            //                      Type = ReferenceType.SecurityScheme,
            //                      Id = "Bearer"
            //                  }
            //              },
            //              Array.Empty<string>()
            //          },
            //    });


            //    config.AddSecurityRequirement(new OpenApiSecurityRequirement
            //    {
            //        {
            //            new OpenApiSecurityScheme
            //            {
            //                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            //            },
            //            new string[] {}
            //        }
            //    });

            //});
        }

        //if (options._userSwaggerOptions != null)
        //{
        //    //if (options._useDefaultSwaggerOptions)
        //    //{
        //    //    throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomSwagger)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultSwagger)}, choose either one or another");
        //    //}

        //    //services.AddSwaggerGen(options._userSwaggerOptions);
        //}

        #endregion

        #region Basic authentication (usually for SharedUI)

        if (options._enableBasicAuthenticationHandler)
        {
            services.AddAuthentication(BasicAuthenticationHandler.Basic)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.Basic, null);
        }

        #endregion

        #region HSTS

        if (options._useDefaultHsts)
        {
            if (!options._isDevelopment)
            {
                // https://aka.ms/aspnetcore-hsts
                services.AddHsts(options =>
                {
                    options.Preload = true;
                    options.IncludeSubDomains = true;
                    options.MaxAge = TimeSpan.FromDays(365);
                });
            }
        }

        if (options._userHstsOptions != null)
        {
            if (options._useDefaultHsts)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomHsts)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultHsts)}, choose either one or another");
            }

            if (!options._isDevelopment)
            {
                // https://aka.ms/aspnetcore-hsts
                services.AddHsts(options._userHstsOptions);
            }
        }

        #endregion

        #region SignalR

        if (options._hubMappings != null)
        {
            services.AddSignalR();
        }

        #endregion

        #region HttpContextAccessor

        if (options._useDefaultHttpContextAccessor)
        {
            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        #endregion

        #region Additional custom validators

        foreach (var assembly in options._assembliesForCustomValidators)
        {
            services.RegisterFluentValidators(assembly);
        }

        #endregion

        #region Localization

        services.AddSingleton(serviceProvicer =>
        {
            var logger = serviceProvicer.GetRequiredService<ILogger<LocalizationCache>>();

            return new LocalizationCache(logger);
        });

        #endregion

        #region DbContexts to be registerd in the DI container

        foreach (var context in options._dbcontexts)
        {
            services.AddScoped(typeof(DbContext), context);
        }

        #endregion  
    }

    public static IApplicationBuilder UseRbkApiCoreSetup(this WebApplication app)
    {
        using (var scope = app.Services.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<RbkApiCoreOptions>();

            #region Global error handler

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            #endregion


            #region Response compression

            if (options._useDefaultCompression || options._userCompressionOptions != null)
            {
                app.UseResponseCompression();
            }

            #endregion

            #region CORS

            if (options._useDefaultCors)
            {
                app.UseCors();
            }

            if (options._userCorsOptions != null)
            {
                app.UseCors(options._defaultCorsPolicy);
            }

            #endregion

            #region HSTS

            if (options._useDefaultHsts || options._userHstsOptions != null)
            {
                app.UseHttpsRedirection();

                if (!options._isDevelopment)
                {
                    app.UseHsts();
                }
            }

            #endregion

            #region Swagger


            //if (options._useDefaultSwaggerOptions || options._userSwaggerOptions != null)
            //{
            //    // app.MapOpenApi();

            //    //if (options._useDefaultSwaggerOptions || options._userSwaggerOptions != null)
            //    //{
            //    //    if (options._forceSwaggerBaseUrl != null)
            //    //    {
            //    //        app.UseSwagger(x =>
            //    //        {
            //    //            x.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            //    //            {
            //    //                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = options._forceSwaggerBaseUrl } };
            //    //            });
            //    //        });
            //    //    }
            //    //    else if (options._pathBase != null)
            //    //    {
            //    //        var basePath = options._pathBase;
            //    //        app.UseSwagger(x =>
            //    //        {
            //    //            x.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
            //    //            {
            //    //                swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer { Url = $"{httpReq.Scheme}://{httpReq.Host.Value}{basePath}" } };
            //    //            });
            //    //        });
            //    //    }
            //    //    else
            //    //    {
            //    //        app.UseSwagger();
            //    //    }
            //    //}

            //    //app.UseSwaggerUI(c =>
            //    //{
            //    //    // c.SwaggerEndpoint("/openapi/v1.json", options._applicationName);
            //    //    c.SwaggerEndpoint("/swagger/identity/swagger.json", "Identity API v1");
            //    //    c.RoutePrefix = "swagger";
            //    //    c.DocExpansion(DocExpansion.Full);
            //    //});
            //}

            #endregion

            #region SPA routes

            // TODO: Needs to be tested with minimal APIs
            //foreach (var route in options._spaRoutes)
            //{
            //    app.MapWhen((context) => context.Request.Path.StartsWithSegments(route.Route), (localAppBuilder) =>
            //    {
            //        localAppBuilder.UseStaticFiles();

            //        localAppBuilder.UseRouting();

            //        localAppBuilder.UseEndpoints(endpoints =>
            //        {
            //            endpoints.MapFallbackToFile(route.FallbackFile);
            //        });
            //    });
            //}

            #endregion

            #region Authorization and Authentication

            app.UseAuthentication();
            app.UseAuthorization();

            #endregion

            #region SPA on root

            // TODO: Needs to be tested with minimal APIs
            //if (options._useSpaOnRoot)
            //{
            //    app.MapWhen((context) =>
            //    {
            //        var isApi = context.Request.Path.StartsWithSegments("/api");
            //        var isSharedUi = context.Request.Path.StartsWithSegments("/shared-ui");
            //        var hasOtherSpaRoutes = false;

            //        foreach (var route in options._spaRoutes)
            //        {
            //            hasOtherSpaRoutes = hasOtherSpaRoutes || context.Request.Path.StartsWithSegments(route.FallbackFile);
            //        }

            //        return !isApi && !hasOtherSpaRoutes && !isSharedUi;
            //    }, (appBuilder) =>
            //    {
            //        Log.Logger.Debug($"Enabling static files for landing page");
            //        app.UseStaticFiles();

            //        Log.Logger.Debug($"Enabling routing for landing page");
            //        appBuilder.UseRouting();

            //        Log.Logger.Debug($"Enabling endpoint fallback for landing page");
            //        appBuilder.UseEndpoints(endpoints =>
            //        {
            //            endpoints.MapFallbackToFile("/index.html");
            //        });
            //    });
            //}

            if (options._useStaticFilesForApi)
            {
                if (options._useSpaOnRoot) throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseStaticFiles)} cannot be used with {nameof(RbkApiCoreOptions.UseSpaOnRoot)}. Static files are automatically enabled for SPAs");

                app.UseStaticFiles();
            }

            #endregion

            #region PathBase

            if (options._pathBase != null)
            {
                app.UsePathBase(options._pathBase);
            }

            #endregion

            // Must go after use authentication
            app.Use(async (ctx, next) =>
            {
                var _ = await ctx.AuthenticateAsync();

                var requestContext = ctx.RequestServices.GetRequiredService<IRequestContext>();

                requestContext.TenantId = ctx.GetTenant();
                requestContext.Username = ctx.GetUsername();
                requestContext.CorrelationId = ctx.Request.Headers["X-Correlation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();
                requestContext.CausationId = ctx.Request.Headers["X-Causation-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();

                await next();
            });
        }
        return app;
    }
}

