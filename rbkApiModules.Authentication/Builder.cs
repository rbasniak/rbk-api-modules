using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Infrastructure;
using System;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Authentication
{
    public static class Builder
    {
        public static void AddRbkApiModulesAuthentication(this IServiceCollection services, IConfigurationSection applicationConfiguration)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(applicationConfiguration[nameof(JwtIssuerOptions.SecretKey)]));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = applicationConfiguration[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = applicationConfiguration[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
                options.AccessTokenLife = int.Parse(applicationConfiguration[nameof(JwtIssuerOptions.AccessTokenLife)]);
                options.RefreshTokenLife = int.Parse(applicationConfiguration[nameof(JwtIssuerOptions.RefreshTokenLife)]);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = applicationConfiguration[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = applicationConfiguration[nameof(JwtIssuerOptions.Audience)],

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
                configureOptions.ClaimsIssuer = applicationConfiguration[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.RegisterApplicationServices(Assembly.GetAssembly(typeof(IJwtFactory)));

            AssemblyScanner
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(CreateUser.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }
    }
}
