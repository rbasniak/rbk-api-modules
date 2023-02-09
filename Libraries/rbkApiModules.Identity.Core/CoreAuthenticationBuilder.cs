using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Reflection;
using System.Text;
using rbkApiModules.Commons.Core;
using Microsoft.AspNetCore.Authentication.Negotiate;

namespace rbkApiModules.Identity.Core;

public static class CoreAuthenticationBuilder
{
    public static void AddRbkAuthentication(this IServiceCollection services, RbkAuthenticationOptions options)
    {
        services.AddMvc(o =>
        {
            var data = new List<Tuple<Type, string>>();

            if (options._disablePasswordReset)
            {
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.RedefinePassword)));
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.SendResetPasswordEmail)));
            }

            if (options._disableEmailConfirmation)
            {
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.ConfirmEmail)));
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.ResendEmailConfirmation)));
            }

            if (options._disablePasswordAuthentication)
            {
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.Login)));
            }

            if (options._disableRefreshToken)
            {
                data.Add(new Tuple<Type, string>(typeof(AuthenticationController), nameof(AuthenticationController.RefreshToken)));
            }

            if (options._ntlmMode == NtlmMode.LoginOnly)
            {
                o.Filters.Add(new NtlmFilter());
            }

            o.Conventions.Add(new RemoveActionConvention(data.ToArray()));
        });


        var serviceProvider = services.BuildServiceProvider();
        var configuration = serviceProvider.GetService<IConfiguration>();

        var authOptions = configuration.GetSection(nameof(JwtIssuerOptions));

        var issuer = authOptions[nameof(JwtIssuerOptions.Issuer)];
        var audience = authOptions[nameof(JwtIssuerOptions.Audience)];
        var signingKeyCode = authOptions[nameof(JwtIssuerOptions.SecretKey)];
        var accessTokenLifeStr = authOptions[nameof(JwtIssuerOptions.AccessTokenLife)];
        var refreshTokenLifeStr = authOptions[nameof(JwtIssuerOptions.RefreshTokenLife)];

        if (String.IsNullOrEmpty(issuer)) throw new ArgumentNullException(nameof(JwtIssuerOptions.Issuer));
        if (String.IsNullOrEmpty(audience)) throw new ArgumentNullException(nameof(JwtIssuerOptions.Audience));
        if (String.IsNullOrEmpty(signingKeyCode)) throw new ArgumentNullException(nameof(JwtIssuerOptions.SecretKey));
        if (String.IsNullOrEmpty(accessTokenLifeStr)) throw new ArgumentNullException(nameof(JwtIssuerOptions.AccessTokenLife));
        if (String.IsNullOrEmpty(refreshTokenLifeStr)) throw new ArgumentNullException(nameof(JwtIssuerOptions.RefreshTokenLife));

        if (!Double.TryParse(accessTokenLifeStr, CultureInfo.InvariantCulture, out var accessTokenLife)) throw new InvalidCastException(nameof(JwtIssuerOptions.AccessTokenLife));
        if (!Double.TryParse(refreshTokenLifeStr, CultureInfo.InvariantCulture, out var refreshTokenLife)) throw new InvalidCastException(nameof(JwtIssuerOptions.RefreshTokenLife));

        if (options._useSymetricEncryptationKey == false && options._useAssymetricEncryptationKey == false)
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

        if (options._ntlmMode != NtlmMode.None)
        {
            services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

            // TODO: Check whether this is really needed
            //services.AddAuthorization(options =>
            //{
            //    options.FallbackPolicy = options.DefaultPolicy;
            //});
        }

        services.RegisterApplicationServices(Assembly.GetAssembly(typeof(IJwtFactory)));

        var emailOptions = configuration.GetSection(nameof(AuthEmailOptions));

        var smtpHost = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SmtpHost)];
        var portStr = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.Port)];
        var useSslStr = emailOptions[nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SSL)];

        var senderName = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Name)];
        var senderEmail = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Email)];
        var senderPassword = emailOptions[nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Password)];

        var mainColor = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.MainColor)];
        var fontColor = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.FontColor)];
        var logo = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.Logo)];
        var supportEmail = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.SuportEmail)];
        var accountDetailsUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.AccountDetailsUrl)];
        var passwordResetUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.PasswordResetUrl)];
        var confirmationSuccessUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationSuccessUrl)];
        var confirmationFailedUrl = emailOptions[nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationFailedUrl)];

        var testEnabledStr = emailOptions[nameof(AuthEmailOptions.TestMode) + ":" + nameof(TestOptions.Enabled)];

        if (String.IsNullOrEmpty(smtpHost)) throw new ArgumentNullException(nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SmtpHost));
        if (String.IsNullOrEmpty(portStr)) throw new ArgumentNullException(nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.Port));
        if (String.IsNullOrEmpty(useSslStr)) throw new ArgumentNullException(nameof(AuthEmailOptions.Server) + ":" + nameof(ServerOptions.SSL));

        if (String.IsNullOrEmpty(senderName)) throw new ArgumentNullException(nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Name));
        if (String.IsNullOrEmpty(senderEmail)) throw new ArgumentNullException(nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Email));
        if (String.IsNullOrEmpty(senderPassword)) throw new ArgumentNullException(nameof(AuthEmailOptions.Sender) + ":" + nameof(SenderOptions.Password));

        if (String.IsNullOrEmpty(mainColor)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.MainColor));
        if (String.IsNullOrEmpty(fontColor)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.FontColor));
        if (String.IsNullOrEmpty(logo)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.Logo));
        if (String.IsNullOrEmpty(supportEmail)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.SuportEmail));
        if (String.IsNullOrEmpty(accountDetailsUrl)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.AccountDetailsUrl));
        if (String.IsNullOrEmpty(passwordResetUrl)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.PasswordResetUrl));
        if (String.IsNullOrEmpty(confirmationSuccessUrl)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationSuccessUrl));
        if (String.IsNullOrEmpty(confirmationFailedUrl)) throw new ArgumentNullException(nameof(AuthEmailOptions.EmailData) + ":" + nameof(EmailContentOptions.ConfirmationFailedUrl));

        if (String.IsNullOrEmpty(testEnabledStr)) throw new ArgumentNullException(nameof(AuthEmailOptions.TestMode) + ":" + nameof(TestOptions.Enabled));

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
                    // TODO: log here
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

        AssemblyScanner
            .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(UserLogin.Command)))
                .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomLoginPolicyValidator)))
        {
            services.AddScoped(typeof(ICustomLoginPolicyValidator), type);
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomClaimHandler)))
        {
            services.AddScoped(typeof(ICustomClaimHandler), type);
        }

        foreach (var type in GetClassesImplementingInterface(typeof(ICustomPasswordPolicyValidator)))
        {
            services.AddScoped(typeof(ICustomPasswordPolicyValidator), type);
        }
    }

    private static Type[] GetClassesImplementingInterface(Type type)
    {
        var currentDomain = AppDomain.CurrentDomain;

        var loadedAssemblies = currentDomain.GetAssemblies();

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

public enum NtlmMode
{
    LoginOnly,
    All,
    None
}