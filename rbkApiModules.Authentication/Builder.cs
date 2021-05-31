using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Utilities.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace rbkApiModules.Authentication
{
    public static class Builder
    {
        public static void AddRbkApiAuthenticationModule(this IServiceCollection services, IConfigurationSection applicationConfiguration)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(applicationConfiguration[nameof(JwtIssuerOptions.SecretKey)]));

            services.AddSingleton(typeof(AuthenticationMailConfiguration));

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
                .FindValidatorsInAssembly(Assembly.GetAssembly(typeof(UserLogin.Command)))
                    .ForEach(result => services.AddScoped(result.InterfaceType, result.ValidatorType));
        }

        public static IApplicationBuilder UseRbkApiAuthenticationModule(this IApplicationBuilder app, Action<AuthenticationModuleOptions> configureOptions)
        {
            var mailConfig = app.ApplicationServices.GetService<AuthenticationMailConfiguration>();

            var options = new AuthenticationModuleOptions(mailConfig);
            configureOptions(options);

            var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

            if (options != null)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    using (var context = scope.ServiceProvider.GetService<DbContext>())
                    {
                        if (options.SeedClaims)
                        {
                            if (options.AuthenticationGroups.Count == 0)
                            {
                                options.AddAuthenticationGroup(String.Empty);
                            }

                            foreach (var group in options.AuthenticationGroups)
                            {
                                var claims = context.Set<Claim>().ToList();

                                if (!claims.Any(x => x.Name == AuthenticationClaims.MANAGE_ROLES && x.AuthenticationGroup == group))
                                {
                                    var description = options.ClaimDescriptions != null && !String.IsNullOrEmpty(options.ClaimDescriptions.ManageRoles)
                                        ? options.ClaimDescriptions.ManageRoles
                                        : AuthenticationClaims.MANAGE_ROLES;

                                    context.Set<Claim>().Add(new Claim(AuthenticationClaims.MANAGE_ROLES, description, group));
                                }

                                if (!claims.Any(x => x.Name == AuthenticationClaims.MANAGE_USER_ROLES && x.AuthenticationGroup == group))
                                {
                                    var description = options.ClaimDescriptions != null && !String.IsNullOrEmpty(options.ClaimDescriptions.ManageUserRoles)
                                        ? options.ClaimDescriptions.ManageUserRoles
                                        : AuthenticationClaims.MANAGE_USER_ROLES;

                                    context.Set<Claim>().Add(new Claim(AuthenticationClaims.MANAGE_USER_ROLES, description, group));
                                }

                                if (!claims.Any(x => x.Name == AuthenticationClaims.OVERRIDE_USER_CLAIMS && x.AuthenticationGroup == group))
                                {
                                    var description = options.ClaimDescriptions != null && !String.IsNullOrEmpty(options.ClaimDescriptions.OverrideUserClaims)
                                        ? options.ClaimDescriptions.OverrideUserClaims
                                        : AuthenticationClaims.OVERRIDE_USER_CLAIMS;

                                    context.Set<Claim>().Add(new Claim(AuthenticationClaims.OVERRIDE_USER_CLAIMS, description, group));
                                }
                            }

                            context.SaveChanges();
                        }
                    }
                }
            }

            return app;
        }
    }
}
