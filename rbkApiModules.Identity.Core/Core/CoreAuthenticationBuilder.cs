using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using rbkApiModules.Commons.Core.Helpers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using rbkApiModules.Commons.Core.Authentication;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;



namespace Microsoft.Extensions.DependencyInjection;

public static class CoreAuthenticationBuilder
{
    public static void AddRbkAuthentication(this IServiceCollection services, RbkAuthenticationOptions authenticationOptions)
    {
        services.AddSingleton(authenticationOptions);
        services.AddSingleton<IOptions<RbkAuthenticationOptions>>(new OptionsWrapper<RbkAuthenticationOptions>(authenticationOptions));

        // Register tenant provider for query filters
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantProvider, HttpContextTenantProvider>();

        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        var authOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        var issuer = authOptions[nameof(JwtIssuerOptions.Issuer)] ?? "Unknown";
        var audience = authOptions[nameof(JwtIssuerOptions.Audience)] ?? "Unknown";
        var signingKeyCode = authOptions[nameof(JwtIssuerOptions.SecretKey)] ?? "Unknown";
        var accessTokenLifeStr = authOptions[nameof(JwtIssuerOptions.AccessTokenLife)] ?? "0";
        var refreshTokenLifeStr = authOptions[nameof(JwtIssuerOptions.RefreshTokenLife)] ?? "0";

        if (String.IsNullOrEmpty(issuer)) throw new ArgumentNullException(nameof(JwtIssuerOptions.Issuer));
        if (String.IsNullOrEmpty(audience)) throw new ArgumentNullException(nameof(JwtIssuerOptions.Audience));
        if (String.IsNullOrEmpty(signingKeyCode)) throw new ArgumentNullException(nameof(JwtIssuerOptions.SecretKey));
        if (String.IsNullOrEmpty(accessTokenLifeStr)) throw new ArgumentNullException(nameof(JwtIssuerOptions.AccessTokenLife));
        if (String.IsNullOrEmpty(refreshTokenLifeStr)) throw new ArgumentNullException(nameof(JwtIssuerOptions.RefreshTokenLife));

        if (!Double.TryParse(accessTokenLifeStr, CultureInfo.InvariantCulture, out var accessTokenLife)) throw new InvalidCastException(nameof(JwtIssuerOptions.AccessTokenLife));
        if (!Double.TryParse(refreshTokenLifeStr, CultureInfo.InvariantCulture, out var refreshTokenLife)) throw new InvalidCastException(nameof(JwtIssuerOptions.RefreshTokenLife));

        if (authenticationOptions._useSymetricEncryptationKey == false && authenticationOptions._useAssymetricEncryptationKey == false)
        {
            throw new InvalidOperationException("You need pick either the symetric or assymetric key option for authentication");
        }

        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(signingKeyCode));

        services.Configure<JwtIssuerOptions>(options =>
        {
            options.Issuer = issuer;
            options.Audience = audience;
            options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            options.AccessTokenLife = accessTokenLife;
            options.RefreshTokenLife = refreshTokenLife;
        });

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions[nameof(JwtIssuerOptions.Issuer)],

            ValidateAudience = true,
            ValidAudience = authOptions[nameof(JwtIssuerOptions.Audience)],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        if (authenticationOptions._appendAuthenticationSchemes)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = authOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });
        }

        foreach (var authenticationScheme in authenticationOptions._extraAuthenticationSchemes)
        {
            authenticationScheme(services.AddAuthentication());
        }

        if (TestingEnvironmentChecker.IsTestingEnvironment && authenticationOptions._loginMode == LoginMode.WindowsAuthentication ||
                authenticationOptions._useMockedWindowsAuthentication && authenticationOptions._loginMode == LoginMode.WindowsAuthentication)
        {
            services.AddAuthentication(MockedWindowsAuthenticationHandler.AuthenticationScheme)
                .AddScheme<TestAuthHandlerOptions, MockedWindowsAuthenticationHandler>(MockedWindowsAuthenticationHandler.AuthenticationScheme, options => { });

            services.AddAuthorization(options =>
            {
                if (TestingEnvironmentChecker.IsTestingEnvironment && authenticationOptions._loginMode == LoginMode.WindowsAuthentication ||
                    authenticationOptions._useMockedWindowsAuthentication && authenticationOptions._loginMode == LoginMode.WindowsAuthentication)
                {
                    var validSchemas = new List<string>
                    {
                        MockedWindowsAuthenticationHandler.AuthenticationScheme,
                        JwtBearerDefaults.AuthenticationScheme,
                    };

                    if (authenticationOptions._addApiKeyAuthentication)
                    {
                        validSchemas.Add(RbkAuthenticationSchemes.API_KEY);
                    }

                    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(validSchemas.ToArray());

                    defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                }

                if (authenticationOptions._addApiKeyAuthentication)
                {
                    options.AddPolicy(RbkAuthenticationSchemes.API_KEY_POLICY, policy =>
                        policy.AddAuthenticationSchemes(RbkAuthenticationSchemes.API_KEY).RequireAuthenticatedUser());
                }
            });
        }
        else
        {
            services.AddAuthorization(options =>
            {
                if (authenticationOptions._addApiKeyAuthentication)
                {
                    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                        JwtBearerDefaults.AuthenticationScheme,
                        RbkAuthenticationSchemes.API_KEY);

                    defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();
                    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

                    options.AddPolicy(RbkAuthenticationSchemes.API_KEY_POLICY, policy =>
                        policy.AddAuthenticationSchemes(RbkAuthenticationSchemes.API_KEY).RequireAuthenticatedUser());
                }
            });
        }

        if (authenticationOptions._addApiKeyAuthentication)
        {
            services.AddMemoryCache();
            services.AddScoped<IApiKeyAuthenticationCacheInvalidation, ApiKeyAuthenticationCacheInvalidation>();
            services.AddScoped<IApiKeyUsageTracker, ApiKeyUsageTracker>();
            services.AddScoped<IApiKeyLastUsedThrottler, ApiKeyLastUsedThrottler>();
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    if (!httpContext.Request.Headers.TryGetValue(RbkAuthenticationSchemes.API_KEY, out var values))
                    {
                        return RateLimitPartition.GetNoLimiter("no-api-key-header");
                    }

                    var raw = values.FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        return RateLimitPartition.GetNoLimiter("empty-api-key-header");
                    }

                    var hash = ApiKeyMaterial.HashRawKey(raw);
                    var memoryCache = httpContext.RequestServices.GetRequiredService<IMemoryCache>();
                    var defaultRpm = authenticationOptions._builtInApiKeyOptions.RequestsPerMinute;
                    if (!memoryCache.TryGetValue(ApiKeyCacheKeys.RateLimitPolicy(hash), out ApiKeyRateLimitPolicy ratePolicy))
                    {
                        ratePolicy = new ApiKeyRateLimitPolicy(defaultRpm, defaultRpm);
                    }

                    var partitionKey = $"{hash}\u001f{ratePolicy.RequestsPerMinute}\u001f{ratePolicy.BurstLimit}";
                    return RateLimitPartition.GetTokenBucketLimiter(partitionKey, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = ratePolicy.BurstLimit,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        TokensPerPeriod = ratePolicy.RequestsPerMinute,
                        AutoReplenishment = true,
                        QueueLimit = 0
                    });
                });
            });

            if (authenticationOptions._apiKeyValidatorType != null)
            {
                services.AddScoped(typeof(IApiKeyValidator), authenticationOptions._apiKeyValidatorType);
            }
        }

        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(IJwtFactory)));

        var emailOptions = configuration.GetSection(nameof(AuthEmailOptions));

        var smtpHost = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SmtpHost)] ?? "";
        var portStr = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.Port)] ?? "-1";
        var useSslStr = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SSL)] ?? "false";

        var senderName = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Name)] ?? "";
        var senderEmail = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Email)] ?? "";
        var senderPassword = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Password)] ?? "";

        var mainColor = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.MainColor)] ?? "";
        var fontColor = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.FontColor)] ?? "";
        var logo = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.Logo)] ?? "";
        var supportEmail = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.SuportEmail)] ?? "";
        var accountDetailsUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.AccountDetailsUrl)] ?? "";
        var passwordResetUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.PasswordResetUrl)] ?? "";
        var confirmationSuccessUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationSuccessUrl)] ?? "";
        var confirmationFailedUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationFailedUrl)] ?? "";

        var testEnabledStr = emailOptions[nameof(AuthEmailOptions.TestMode) + ":" + nameof(TestOptions.Enabled)] ?? "false";

        if (!Int32.TryParse(portStr, out var port)) throw new InvalidCastException(nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.Port));
        if (!Boolean.TryParse(useSslStr, out var useSsl)) throw new InvalidCastException(nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SSL));
        if (!Boolean.TryParse(testEnabledStr, out var testEnabeld)) throw new InvalidCastException(nameof(AuthEmailOptions.TestMode) + ":" + nameof(TestOptions.Enabled));

        string testOutputFolder = null;

        if (testEnabeld)
        {
            testOutputFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "mail_test");

            if (!Directory.Exists(testOutputFolder))
            {
                try
                {
                    Directory.CreateDirectory(testOutputFolder);
                }
                catch (Exception ex)
                {
                    throw new DirectoryNotFoundException(testOutputFolder);
                }
            }
        }

        services.Configure<AuthEmailOptions>(options =>
        {
            options.Server = new ServerOptions
            {
                SmtpHost = smtpHost,
                SSL = useSsl,
                Port = port,
            };

            options.Sender = new SenderOptions
            {
                Email = senderEmail,
                Name = senderName,
                Password = senderPassword,
            };

            options.EmailData = new EmailContentOptions
            {
                AccountDetailsUrl = accountDetailsUrl,
                ConfirmationFailedUrl = confirmationFailedUrl,
                ConfirmationSuccessUrl = confirmationSuccessUrl,
                FontColor = fontColor,
                Logo = logo,
                MainColor = mainColor,
                PasswordResetUrl = passwordResetUrl,
                SuportEmail = supportEmail
            };

            options.TestMode = new TestOptions
            {
                Enabled = testEnabeld,
                OutputFolder = testOutputFolder
            };
        });

        services.AddScoped<IAuthenticationMailService, DefaultAuthenticationMailService>();

        services.RegisterFluentValidators(Assembly.GetAssembly(typeof(UserLogin.Request)));

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomLoginPolicyValidator)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(ICustomLoginPolicyValidator) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(ICustomLoginPolicyValidator), type);
            }
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomClaimHandler)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(ICustomClaimHandler) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(ICustomClaimHandler), type);
            }
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomUserPostProcessor)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(ICustomUserPostProcessor) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(ICustomUserPostProcessor), type);
            }
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomPasswordPolicyValidator)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(ICustomPasswordPolicyValidator) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(ICustomPasswordPolicyValidator), type);
            }
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomUserMetadataValidator)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(ICustomUserMetadataValidator) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(ICustomUserMetadataValidator), type);
            }
        }

        foreach (var type in GetClassesImplementingInterface(typeof(IUserMetadataService)))
        {
            // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
            if (services.None(x => x.ServiceType == typeof(IUserMetadataService) && x.ImplementationType == type))
            {
                services.AddScoped(typeof(IUserMetadataService), type);
            }
        }

        // Avoiid duplicated registrations. Depending on naming convention used, it could be already registered by the generic .AddApplicationServices() method
        if (services.None(x => x.ServiceType == typeof(IAvatarStorage) && x.ImplementationType == authenticationOptions._customAvatarStorageType))
        {
            services.AddScoped(typeof(IAvatarStorage), authenticationOptions._customAvatarStorageType);
        }
    }

    private static Type[] GetClassesImplementingInterface(Type type)
    {
        var currentDomain = AppDomain.CurrentDomain;

        var loadedAssemblies = currentDomain.GetAssemblies().Where(x => !x.FullName.StartsWith("Microsoft") && !x.FullName.StartsWith("System"));

        List<Type> result = new List<Type>();

        foreach (var assembly in loadedAssemblies)
        {
            var implementingTypes = assembly.GetTypes()
                .Where(myType => ((myType.IsClass && !myType.IsAbstract) || myType.IsEnum)
                        && myType.GetInterfaces().Any(x => x.FullName == type.FullName))
                .ToArray();

            result.AddRange(implementingTypes);
        }

        return result.ToArray();
    } 
}

