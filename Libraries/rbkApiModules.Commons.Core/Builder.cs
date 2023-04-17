using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.IO.Compression;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using Serilog;
using rbkApiModules.Commons.Core.Pipelines;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.Negotiate;
using rbkApiModules.Commons.Core.Utilities;
using System.Globalization;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Logging;

[assembly: InternalsVisibleTo("rbkApiModules.Commons.Relational")]

namespace rbkApiModules.Commons.Core;


public class RbkApiCoreOptions
{
    #region IHttpContextAccessor 

    internal bool _useDefaultHttpContextAccessor = false;

    public RbkApiCoreOptions UseHttpContextAccessor()
    {
        _useDefaultHttpContextAccessor = true;
        return this;
    }

    #endregion

    #region Response compression

    internal bool _useDefaultCompression = false;
    internal Action<ResponseCompressionOptions> _userCompressionOptions = null;
    internal Action<GzipCompressionProviderOptions> _userProviderCompressionOptions = null;

    public RbkApiCoreOptions UseDefaultCompression()
    {
        _useDefaultCompression = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomCompression(Action<ResponseCompressionOptions> compressionOptions, Action<GzipCompressionProviderOptions> providerOptions = null)
    {
        if (compressionOptions == null) throw new ArgumentNullException(nameof(compressionOptions));
        _userCompressionOptions = compressionOptions;
        _userProviderCompressionOptions = providerOptions;
        return this;
    }

    #endregion

    #region Service registration

    internal List<Assembly> _assembliesForServices = new List<Assembly>();

    public RbkApiCoreOptions RegisterServices(params Assembly[] assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        _assembliesForServices.AddRange(assemblies);
        return this;
    }

    #endregion

    #region Http client

    internal bool _useDefaultHttpClient = false;

    public RbkApiCoreOptions UseDefaultHttpClient()
    {
        _useDefaultHttpClient = true;
        return this;
    }

    #endregion

    #region Memory cache

    internal bool _useDefaultMemoryCache = false;
    internal Action<MemoryCacheOptions> _userMemoryCacheOptions = null;

    public RbkApiCoreOptions UseDefaultMemoryCache()
    {
        _useDefaultMemoryCache = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomMemoryCache(Action<MemoryCacheOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _userMemoryCacheOptions = options;
        return this;
    }

    #endregion

    #region AutoMapper

    internal bool _useDefaultAutoMapper = false;
    internal MapperConfiguration _userAutoMapperOptions = null;
    internal List<Assembly> _assembliesForMapping = new List<Assembly>();
    internal List<Profile> _manuallyAddedMappingProfiles = new List<Profile>();

    public RbkApiCoreOptions UseDefaultAutoMapper()
    {
        _useDefaultAutoMapper = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomAutoMapper(MapperConfiguration configuration)
    {
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));
        _userAutoMapperOptions = configuration;
        return this;
    }

    public RbkApiCoreOptions RegisterMappings(params Profile[] profiles)
    {
        if (profiles == null) throw new ArgumentNullException(nameof(profiles));
        _manuallyAddedMappingProfiles.AddRange(profiles);
        return this;
    }

    public RbkApiCoreOptions RegisterMappings(params Assembly[] assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        _assembliesForMapping.AddRange(assemblies);
        return this;
    }

    #endregion

    #region Https redirection

    internal bool _useDefaultHttpsRedirection = false;
    internal Action<HttpsRedirectionOptions> _userHttpsRedirectionOptions = null;

    public RbkApiCoreOptions UseDefaultHttpsRedirection()
    {
        _useDefaultHttpsRedirection = true;

        return this;
    }

    public RbkApiCoreOptions UseCustomHttpsRedirection(Action<HttpsRedirectionOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _userHttpsRedirectionOptions = options;
        return this;
    }

    #endregion

    #region API controllers

    internal bool _useDefaultApiControllers = false;
    internal Action<MvcOptions> _userApiControllersOptions = null;
    internal List<IActionFilter> _mvcFilters = new List<IActionFilter>();

    public RbkApiCoreOptions UseDefaultApiControllers()
    {
        _useDefaultApiControllers = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomApiControllers(Action<MvcOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _userApiControllersOptions = options;
        return this;
    }

    public RbkApiCoreOptions UseCustomMvcFilters(params IActionFilter[] filters)
    {
        if (filters == null) throw new ArgumentNullException(nameof(filters));
        _mvcFilters.AddRange(filters);
        return this;
    }

    #endregion

    #region API routing

    internal bool _useDefaultApiRouting = false;
    internal Action<RouteOptions> _userApiRoutingOptions = null;

    public RbkApiCoreOptions UseDefaultApiRouting()
    {
        _useDefaultApiRouting = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomApiRouting(Action<RouteOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _userApiRoutingOptions = options;
        return this;
    }

    #endregion

    #region CORS

    internal bool _useDefaultCors = false;
    internal string _defaultCorsPolicy = String.Empty;
    internal Action<CorsOptions> _userCorsOptions = null;

    public RbkApiCoreOptions UseDefaultCors()
    {
        _useDefaultCors = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomCors(string policyName, Action<CorsOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (String.IsNullOrEmpty(policyName)) throw new ArgumentNullException(nameof(policyName));
        _userCorsOptions = options;
        _defaultCorsPolicy = policyName;
        return this;
    }

    #endregion

    #region Swagger

    internal bool _useDefaultSwaggerOptions = false;
    internal Action<SwaggerGenOptions> _userSwaggerOptions = null;
    internal string _applicationName = "Default";

    public RbkApiCoreOptions UseDefaultSwagger(string applicationName)
    {
        _useDefaultSwaggerOptions = true;
        _applicationName = applicationName;
        return this;
    }

    public RbkApiCoreOptions UseCustomSwagger(Action<SwaggerGenOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _userSwaggerOptions = options;
        return this;
    }

    #endregion

    #region HSTS

    internal bool _isDevelopment = false;
    internal bool _useDefaultHsts = false;
    internal Action<HstsOptions> _userHstsOptions = null;

    public RbkApiCoreOptions UseDefaultHsts(bool isDevelopment)
    {
        _isDevelopment = isDevelopment;
        _useDefaultHsts = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomHsts(Action<HstsOptions> options, bool isDevelopment)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        _isDevelopment = isDevelopment;
        _userHstsOptions = options;
        return this;
    }

    #endregion

    #region ModelStateInvalidFilter suppression

    internal bool _suppressModelStateInvalidFilter = false;

    public RbkApiCoreOptions SuppressModelStateInvalidFilter()
    {
        _suppressModelStateInvalidFilter = true;
        return this;
    }

    #endregion

    #region Basic authentication handler

    internal bool _enableBasicAuthenticationHandler = false;

    public RbkApiCoreOptions EnableBasicAuthenticationHandler()
    {
        _enableBasicAuthenticationHandler = true;
        return this;
    }

    #endregion

    #region FluentValidation global behavior

    internal bool _useDefaulFluentValidationGlobalBehavior = false;

    public RbkApiCoreOptions UseDefaulFluentValidationGlobalBehavior()
    {
        _useDefaulFluentValidationGlobalBehavior = true;
        return this;
    }

    #endregion

    #region MediatR assemblies

    internal List<Assembly> _assembliesForMediatR = new List<Assembly>();

    public RbkApiCoreOptions RegisterMediatR(params Assembly[] assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        _assembliesForMediatR.AddRange(assemblies);
        return this;
    }

    #endregion

    #region Custom validators assemblies

    internal List<Assembly> _assembliesForCustomValidators = new List<Assembly>();

    public RbkApiCoreOptions RegisterAdditionalValidators(params Assembly[] assemblies)
    {
        if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
        _assembliesForCustomValidators.AddRange(assemblies);
        return this;
    }

    #endregion

    #region MediatR pipelines

    internal List<Type> _pipelines = new List<Type>();

    public RbkApiCoreOptions UseDefaultPipelines()
    {
        _pipelines.Add(typeof(LoggingBehavior<,>));
        _pipelines.Add(typeof(ExceptionHandlerBehavior<,>));
        _pipelines.Add(typeof(PerformanceBehaviour<,>));
        _pipelines.Add(typeof(AuthCommandPreProcessingBehavior<,>));
        _pipelines.Add(typeof(FailFastRequestBehavior<,>));
        _pipelines.Add(typeof(CqrsReplicaBehavior<,>));
        _pipelines.Add(typeof(TransactionBehavior<,>));
        _pipelines.Add(typeof(AuditingBehavior<,>));

        return this;
    }

    public RbkApiCoreOptions UseCustomPipelines(List<Type> pipelines)
    {
        if (pipelines == null) throw new ArgumentNullException(nameof(pipelines));

        _pipelines = pipelines;

        return this;
    }

    public RbkApiCoreOptions SuppressPipeline(Type type)
    {
        if (type == null) throw new ArgumentNullException(nameof(type));

        if (!_pipelines.Remove(type))
        {
            throw new InvalidOperationException($"The {type.Name} pipeline is not registered. Please use {nameof(UseDefaultPipelines)} or {nameof(UseCustomPipelines)} before calling {nameof(SuppressPipeline)}");
        }

        return this;
    }

    #endregion

    #region SignalR hubs

    internal Action<IEndpointRouteBuilder> _hubMappings = null;

    public RbkApiCoreOptions MapSignalR(Action<IEndpointRouteBuilder> mappings)
    {
        if (mappings == null) throw new ArgumentNullException(nameof(mappings));
        _hubMappings = mappings;
        return this;
    }

    #endregion

    #region Static files (when not using SPA on root)

    internal bool _useStaticFilesForApi = false;

    public RbkApiCoreOptions UseStaticFiles()
    {
        _useStaticFilesForApi = true;
        return this;
    }

    #endregion

    #region SPA routes

    internal List<(string Route, string FallbackFile)> _spaRoutes = new List<(string, string)>();
    internal bool _useSpaOnRoot = false;

    public RbkApiCoreOptions MapSpas(params string[] routes)
    {
        if (routes == null) throw new ArgumentNullException(nameof(routes));
        foreach (var route in routes)
        {
            var sanitizedRoute = $"/{route.Trim('/').ToLower()}";
            _spaRoutes.Add(new(sanitizedRoute, $"{sanitizedRoute}/index.html"));
        }
        return this;
    }
    public RbkApiCoreOptions UseSpaOnRoot()
    {
        _useSpaOnRoot = true;
        return this;
    }

    #endregion

    #region Simple CQRS

    internal bool _useSimpleCqrsBehavior = false;
    // internal Action<SimpleCqrsBehaviorOptions> _configureCqrsAction;

    //public RbkApiCoreOptions UseSimpleCqrs(Action<SimpleCqrsBehaviorOptions> options)
    //{
    //    _useSimpleCqrsBehavior = true;
    //    _configureCqrsAction = options;

    //    return this;
    //}

    #endregion

    #region Localization

    internal string _defaultLocalization = "en-us";

    public RbkApiCoreOptions UseDefaultLocalizationLanguage(string code)
    {
        if (code == null) throw new ArgumentNullException(nameof(code));

        _defaultLocalization = code;
        return this;
    }

    #endregion
}

public static class CommonsCoreBuilder
{
    public static void AddRbkApiCoreSetup(this IServiceCollection services, RbkApiCoreOptions options)
    {
        services.AddSingleton(options);

        Log.Logger.Debug($"Start configuring Core API capabilities");

        // Log.Logger.Debug($"Registering {nameof(IInMemoryDatabase)}");
        // services.AddScoped<IInMemoryDatabase, InMemoryDatabase>();

        #region Logging basic setup

        services.AddSingleton<HttpContextEnricher>(); 

        #endregion

        #region Response Compression

        if (options._useDefaultCompression)
        {
            Log.Logger.Debug($"Using default response compression settings");

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
        }

        if (options._userCompressionOptions != null)
        {
            Log.Logger.Debug($"Using custom response compression settings");

            if (options._useDefaultCompression)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomCompression)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultCompression)}, choose either one or another");
            }

            services.AddResponseCompression(options._userCompressionOptions);

            if (options._userProviderCompressionOptions != null)
            {
                Log.Logger.Debug($"Using custom response compression provider settings");

                services.Configure<GzipCompressionProviderOptions>(options._userProviderCompressionOptions);
            }
            else
            {
                Log.Logger.Debug($"Using default response compression provider settings");

                services.Configure<GzipCompressionProviderOptions>(options => options.Level = CompressionLevel.Optimal);
            }
        }

        #endregion

        #region Application services

        Log.Logger.Debug($"Registering services within {Assembly.GetExecutingAssembly().GetName().Name}");

        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(CommonsCoreBuilder)));

        foreach (var assembly in options._assembliesForServices)
        {
            Log.Logger.Debug($"Registering services within {assembly.GetName().Name}");

            services.RegisterApplicationServices(options._assembliesForServices.ToArray());
        }

        #endregion

        #region Basic HttpClient

        if (options._useDefaultHttpClient)
        {
            Log.Logger.Debug($"Using basic HttpClient settings");

            services.AddHttpClient();
        }

        #endregion

        #region Memory cache

        if (options._useDefaultMemoryCache)
        {
            Log.Logger.Debug($"Using default in-memory cache settings");

            services.AddMemoryCache();
        }

        if (options._userMemoryCacheOptions != null)
        {
            Log.Logger.Debug($"Using custom in-memory cache settings");

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
            Log.Logger.Debug($"Registering IHttpContextAccessor");

            services.TryAddTransient<IHttpContextAccessor, HttpContextAccessor>();
        }

        #endregion

        #region AutoMapper configuration

        MapperConfiguration mapperConfiguration = null;

        if (options._useDefaultAutoMapper)
        {
            Log.Logger.Debug($"Using default AutoMapper configuration");

            mapperConfiguration = new MapperConfiguration(cfg =>
            {
                if (options._assembliesForMapping.Count > 0)
                {
                    cfg.ApplyProfiles(options._assembliesForMapping.ToArray());
                }

                cfg.CreateMap<Guid?, string>().ConvertUsing(g => g.HasValue ? g.Value.ToString().ToLower() : null);
                cfg.CreateMap<Guid, string>().ConvertUsing(g => g.ToString().ToLower());

                foreach (var profile in options._manuallyAddedMappingProfiles)
                {
                    Log.Logger.Debug($"Adding extra mapping profile: {profile.GetType().FullName}");

                    cfg.AddProfile(profile);
                }
            });
        }

        if (options._userAutoMapperOptions != null)
        {
            Log.Logger.Debug($"Using custom AutoMapper configuration");

            if (options._useDefaultAutoMapper)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomAutoMapper)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultAutoMapper)}, choose either one or another");
            }

            if (options._assembliesForMapping.Count > 0 || options._manuallyAddedMappingProfiles.Count > 0)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.RegisterMappings)} cannot be used with {nameof(RbkApiCoreOptions.UseCustomAutoMapper)}, choose either one or another");
            }

            mapperConfiguration = options._userAutoMapperOptions;
        }

        if (mapperConfiguration != null)
        {
            Log.Logger.Debug($"Finishing AutoMapper configuration");

            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
        else
        {
            Log.Logger.Debug($"AutoMapper configuration mode was not chosen, skipping AutoMapper registration in the container");
        }

        #endregion

        #region HTTPS redirection

        if (options._useDefaultHttpsRedirection)
        {
            Log.Logger.Debug($"Using default HTTPS redirection");

            services.AddHttpsRedirection(options =>
            {
                options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
                options.HttpsPort = 443;
            });
        }

        if (options._userHttpsRedirectionOptions != null)
        {
            Log.Logger.Debug($"Using custom in-memory cache settings");

            if (options._useDefaultHttpsRedirection)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseDefaultHttpsRedirection)} cannot be used with {nameof(RbkApiCoreOptions.UseCustomHttpsRedirection)}, choose either one or another");
            }

            services.AddHttpsRedirection(options._userHttpsRedirectionOptions);
        }

        #endregion

        #region API controllers

        if (options._useDefaultApiControllers)
        {
            Log.Logger.Debug($"Using default API controller settings");

            services.AddControllers(mvcOptions =>
            {
                foreach (var filter in options._mvcFilters)
                {
                    Log.Logger.Debug($"Registering new MVC filter: {filter.GetType().FullName}");

                    mvcOptions.Filters.Add(filter);
                }
            });
        }

        if (options._userApiControllersOptions != null)
        {
            Log.Logger.Debug($"Using custom API controller settings");

            if (options._useDefaultApiControllers)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomApiControllers)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultApiControllers)}, choose either one or another");
            }

            if (options._mvcFilters.Count > 0)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomMvcFilters)} cannot be used with {nameof(RbkApiCoreOptions.UseCustomApiControllers)}, choose either one or another");
            }

            services.AddControllers(options._userApiControllersOptions);
        }

        #endregion

        #region API routing

        if (options._useDefaultApiRouting)
        {
            Log.Logger.Debug($"Using default API routing settings");

            services.AddRouting(options =>
            {
                options.LowercaseQueryStrings = true;
                options.LowercaseUrls = true;
            });

            if (!options._isDevelopment)
            {
                services.AddMvc(options =>
                {
                    // TODO: Pensar como remover o code generator em production
                    //options.Conventions.Add(new RemoveActionConvention(new[] {
                    //    new Tuple<Type, string>(typeof(CodeGeneratorController), nameof(CodeGeneratorController.Get))
                    //}));
                });
            }

            // TODO: Hide these on swagger only
            //services.AddMvc(options =>
            //{
            //    options.Conventions.Add(new RemoveActionConvention(new[] {
            //        new Tuple<Type, string>(typeof(SharedUIController), nameof(SharedUIController.GetFilterData)),
            //        new Tuple<Type, string>(typeof(SharedUIController), nameof(SharedUIController.Login)),
            //    }));
            //});
        }

        if (options._userApiRoutingOptions != null)
        {
            Log.Logger.Debug($"Using custom API routing settings");

            if (options._useDefaultApiRouting)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomApiRouting)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultApiRouting)}, choose either one or another");
            }

            services.AddRouting(options._userApiRoutingOptions);
        }

        #endregion

        #region CORS

        if (options._useDefaultCors)
        {
            Log.Logger.Debug($"Using default CORS settings");

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
            Log.Logger.Debug($"Using custom CORS settings");

            if (options._useDefaultCors)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomCors)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultCors)}, choose either one or another");
            }

            services.AddCors(options._userCorsOptions);
        }

        #endregion

        #region Swagger

        var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

        if (options._useDefaultSwaggerOptions)
        {
            if (!File.Exists(xmlPath))
            {
                Log.Logger.Fatal("Missing API Documentation file for Swagger");
            }

            services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = options._applicationName });

                if (File.Exists(xmlPath))
                {
                    config.IncludeXmlComments(xmlPath);
                }

                config.CustomSchemaIds(x => x.FullName.Replace("+", "."));

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
        }

        if (options._userSwaggerOptions != null)
        {
            Log.Logger.Debug($"Using custom Swagger settings");

            if (options._useDefaultSwaggerOptions)
            {
                throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseCustomSwagger)} cannot be used with {nameof(RbkApiCoreOptions.UseDefaultSwagger)}, choose either one or another");
            }

            services.AddSwaggerGen(options._userSwaggerOptions);
        }

        #endregion

        #region Basic authentication (usually for SharedUI)

        if (options._enableBasicAuthenticationHandler)
        {
            services.AddAuthentication(BasicAuthenticationHandler.Basic)
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.Basic, null);
        }

        #endregion

        #region Suppress ModelStateInvalidFilter

        if (options._suppressModelStateInvalidFilter)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }

        #endregion

        #region HSTS

        if (options._useDefaultHsts)
        {
            Log.Logger.Debug($"Using default HSTS settings (production only)");

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
            Log.Logger.Debug($"Using custom HSTS settings(production only)");

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

        #region MediatR

        Log.Logger.Debug($"Looking for MediatR command validators and registering them");

        foreach (var assembly in options._assembliesForMediatR)
        {
            Log.Logger.Debug($"Registering validators in '{assembly.FullName}'");

            services.RegisterFluentValidators(assembly); 
        }

        foreach (var type in options._pipelines)
        {
            Log.Logger.Debug($"Registering pipeline: {type.Name}");

            services.AddScoped(typeof(IPipelineBehavior<,>), type);
        }

        Log.Logger.Debug($"Registering MediatR commands");

        if (options._assembliesForMediatR.Count > 0)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(options._assembliesForMediatR.ToArray()));
        }

        #endregion

        #region FluentValidation global behavior

        if (options._useDefaulFluentValidationGlobalBehavior)
        {
            ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Continue;
        }

        #endregion

        #region SignalR

        if (options._hubMappings != null)
        {
            Log.Logger.Debug($"Using SignalR");

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

        Log.Logger.Debug($"Looking for additional custom validators and registering them");

        foreach (var assembly in options._assembliesForCustomValidators)
        {
            Log.Logger.Debug($"Registering validators in '{assembly.FullName}'");

            services.RegisterFluentValidators(assembly);
        }

        #endregion

        #region Simple CQRS

        // Log.Logger.Debug($"Enabling simple CQRS behavior");

        //if (options._useSimpleCqrsBehavior)
        //{
        //    var cqrsOptions = new SimpleCqrsBehaviorOptions();

        //    options._configureCqrsAction(cqrsOptions);

        //    services.AddSingleton(cqrsOptions);
        //}

        #endregion

        #region Localization

        services.AddSingleton(new LocalizationCache());

        #endregion

        Log.Logger.Debug($"Done configuring Core API capabilities");
    }

    public static IApplicationBuilder UseRbkApiCoreSetup(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var options = scope.ServiceProvider.GetService<RbkApiCoreOptions>();

            #region Response compression

            if (options._useDefaultCompression || options._userCompressionOptions != null)
            {
                Log.Logger.Debug($"Enabling response compression");

                app.UseResponseCompression();
            }

            #endregion

            #region HSTS

            if (options._useDefaultHsts || options._userHstsOptions != null)
            {
                Log.Logger.Debug($"Enabling HSTS");

                app.UseHttpsRedirection();

                if (!options._isDevelopment)
                {
                    app.UseHsts();
                }
            }

            #endregion

            #region Swagger

            if (options._useDefaultSwaggerOptions || options._userSwaggerOptions != null)
            {
                Log.Logger.Debug($"Enabling Swagger");

                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("v1/swagger.json", "Default");
                    c.RoutePrefix = "swagger";
                    c.DocExpansion(DocExpansion.None);
                });
            }

            #endregion

            #region SPA routes

            foreach (var route in options._spaRoutes)
            {
                Log.Logger.Debug($"Preparing to setup SPA on {route.Route} with the following fallback file: {route.FallbackFile}");

                app.MapWhen((context) => context.Request.Path.StartsWithSegments(route.Route), (localAppBuilder) =>
                {
                    Log.Logger.Debug($"Enabling static files for {route.Route}");
                    localAppBuilder.UseStaticFiles();

                    Log.Logger.Debug($"Enabling routing for {route.Route}");
                    localAppBuilder.UseRouting();

                    Log.Logger.Debug($"Enabling endpoint fallback for {route.Route}");
                    localAppBuilder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile(route.FallbackFile);
                    });
                });
            }

            #endregion

            #region API routes

            //app.MapWhen((context) => context.Request.Path.StartsWithSegments("/api/authentication/login"), (appBuilder) =>
            //{
            //    Log.Logger.Debug($"Enabling routing for API");
            //    appBuilder.UseRouting();

            //    if (options._defaultCorsPolicy != null)
            //    {
            //        Log.Logger.Debug($"Enabling CORS for API with the specified policy: {options._defaultCorsPolicy}");
            //        appBuilder.UseCors(options._defaultCorsPolicy);
            //    }
            //    else
            //    {
            //        Log.Logger.Debug($"Enabling CORS for API with defaults");
            //        appBuilder.UseCors();
            //    }

            //    Log.Logger.Debug($"Enabling authentication for API");
            //    appBuilder.UseAuthentication();

            //    Log.Logger.Debug($"Enabling authorization for API");
            //    appBuilder.UseAuthorization();

            //    Log.Logger.Debug($"Enabling endpoints for API");
            //    appBuilder.UseEndpoints(endpoints =>
            //    {
            //        Log.Logger.Debug($"Enabling controller mapping for API");
            //        endpoints.MapControllers();

            //        if (options._hubMappings != null)
            //        {
            //            Log.Logger.Debug($"Enabling SignalR hubs for API");
            //            options._hubMappings(endpoints);
            //        }
            //    });
            //});

            app.MapWhen((context) => context.Request.Path.StartsWithSegments("/api"), (appBuilder) =>
            {
                Log.Logger.Debug($"Enabling routing for API");
                appBuilder.UseRouting();

                if (!String.IsNullOrEmpty(options._defaultCorsPolicy))
                {
                    Log.Logger.Debug($"Enabling CORS for API with the specified policy: '{options._defaultCorsPolicy}'");
                    appBuilder.UseCors(options._defaultCorsPolicy);
                }
                else
                {
                    Log.Logger.Debug($"Enabling CORS for API with defaults");
                    appBuilder.UseCors();
                }

                Log.Logger.Debug($"Enabling authentication for API");
                appBuilder.UseAuthentication();

                Log.Logger.Debug($"Enabling authorization for API");
                appBuilder.UseAuthorization();

                Log.Logger.Debug($"Enabling endpoints for API");
                appBuilder.UseEndpoints(endpoints =>
                {
                    Log.Logger.Debug($"Enabling controller mapping for API");
                    endpoints.MapControllers();

                    if (options._hubMappings != null)
                    {
                        Log.Logger.Debug($"Enabling SignalR hubs for API");
                        options._hubMappings(endpoints);
                    }
                });
            });

            #endregion

            #region SPA on root

            if (options._useSpaOnRoot)
            {
                app.MapWhen((context) =>
                {
                    var isApi = context.Request.Path.StartsWithSegments("/api");
                    var isSharedUi = context.Request.Path.StartsWithSegments("/shared-ui");
                    var hasOtherSpaRoutes = false;

                    foreach (var route in options._spaRoutes)
                    {
                        hasOtherSpaRoutes = hasOtherSpaRoutes || context.Request.Path.StartsWithSegments(route.FallbackFile);
                    }

                    return !isApi && !hasOtherSpaRoutes && !isSharedUi;
                }, (appBuilder) =>
                {
                    Log.Logger.Debug($"Enabling static files for landing page");
                    app.UseStaticFiles();

                    Log.Logger.Debug($"Enabling routing for landing page");
                    appBuilder.UseRouting();

                    Log.Logger.Debug($"Enabling endpoint fallback for landing page");
                    appBuilder.UseEndpoints(endpoints =>
                    {
                        endpoints.MapFallbackToFile("/index.html");
                    });
                });
            }
            else
            {
                if (options._useStaticFilesForApi)
                {
                    if (options._useSpaOnRoot) throw new InvalidOperationException($"{nameof(RbkApiCoreOptions.UseStaticFiles)} cannot be used with {nameof(RbkApiCoreOptions.UseSpaOnRoot)}. Static files are automatically enabled for SPAs");

                    Log.Logger.Debug($"Enabling static files for API");
                    app.UseStaticFiles();

                }
            }

            #endregion
        }
        return app;
    }

    //public static IApplicationBuilder UseSimpleCqrs(this IApplicationBuilder app)
    //{
    //    using (var scope = app.ApplicationServices.CreateScope())
    //    {
    //        var options = scope.ServiceProvider.GetService<RbkApiCoreOptions>();

    //        #region Simple CQRS

    //        if (options._useSimpleCqrsBehavior)
    //        {
    //            var context = scope.ServiceProvider.GetRequiredService<IInMemoryDatabase>();

    //            if (context == null) throw new InvalidOperationException($"It seems that {nameof(IInMemoryDatabase)} is not properly setup");

    //            var cqrsOptions = scope.ServiceProvider.GetRequiredService<SimpleCqrsBehaviorOptions>();

    //            foreach (var item in cqrsOptions._initializationFunctions)
    //            {
    //                context.Initialize(item.Key, item.Value);
    //            }
    //        }
    //        #endregion
    //    }

    //    return app;
    //}
}