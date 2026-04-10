using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

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

    #region CORS

    internal bool _useDefaultCors = false;
    internal string _defaultCorsPolicy = string.Empty;
    internal Action<CorsOptions> _userCorsOptions = null;

    public RbkApiCoreOptions UseDefaultCors()
    {
        _useDefaultCors = true;
        return this;
    }

    public RbkApiCoreOptions UseCustomCors(string policyName, Action<CorsOptions> options)
    {
        if (options == null) throw new ArgumentNullException(nameof(options));
        if (string.IsNullOrEmpty(policyName)) throw new ArgumentNullException(nameof(policyName));
        _userCorsOptions = options;
        _defaultCorsPolicy = policyName;
        return this;
    }

    #endregion

    #region Swagger

    internal bool _useDefaultSwaggerOptions = false;
    // internal Action<SwaggerGenOptions> _userSwaggerOptions = null;
    internal string _applicationName = "Default";
    internal string _forceSwaggerBaseUrl = null;

    public RbkApiCoreOptions UseDefaultSwagger(string applicationName)
    {
        _useDefaultSwaggerOptions = true;
        _applicationName = applicationName;
        return this;
    }

    //public RbkApiCoreOptions UseDefaultSwagger(string applicationName, string forceSwaggerBaseUrl)
    //{
    //    _forceSwaggerBaseUrl = forceSwaggerBaseUrl;
    //    return UseDefaultSwagger(applicationName);
    //}

    //public RbkApiCoreOptions UseCustomSwagger(Action<SwaggerGenOptions> options)
    //{
    //    if (options == null) throw new ArgumentNullException(nameof(options));
    //    _userSwaggerOptions = options;
    //    return this;
    //}

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

    #region Basic authentication handler

    internal bool _enableBasicAuthenticationHandler = false;

    public RbkApiCoreOptions EnableBasicAuthenticationHandler()
    {
        _enableBasicAuthenticationHandler = true;
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

    #region Localization

    internal string _defaultLocalization = "en-us";

    public RbkApiCoreOptions UseDefaultLocalizationLanguage(string code)
    {
        if (code == null) throw new ArgumentNullException(nameof(code));

        _defaultLocalization = code;
        return this;
    }

    #endregion

    #region PathBase

    internal string _pathBase = null;

    public RbkApiCoreOptions UsePathBase(string pathBase)
    {
        if (pathBase == null) throw new ArgumentNullException(nameof(pathBase));

        _pathBase = "/" + pathBase.Trim('/');
        return this;
    }

    #endregion


    #region DbContexts to be registerd in the DI container

    internal List<Type> _dbcontexts = [];

    public RbkApiCoreOptions RegisterDbContext<TDbContext>()
    {
        _dbcontexts.Add(typeof(TDbContext));
        return this;
    }

    #endregion
}