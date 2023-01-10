using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational.CQRS;

namespace rbkApiModules.Identity.Relational;

public static class Builder
{
    public static IServiceCollection AddRbkRelationalAuthentication(this IServiceCollection services, Action<RbkAuthenticationOptions> configureOptions)
    {
        var options = new RbkAuthenticationOptions();
        configureOptions(options);

        services.AddRbkAuthentication(options);

        services.AddScoped<IAuthService, RelationalAuthService>();
        services.AddScoped<IRolesService, RelationalRolesService>();
        services.AddScoped<IClaimsService, RelationalClaimsService>();
        services.AddScoped<ITenantsService, RelationalTenantsService>();

        if (options._tenantPostCreationActionType != null)
        {
            services.AddScoped(typeof(ITenantPostCreationAction), options._tenantPostCreationActionType);
        }
        else
        {
            services.AddScoped<ITenantPostCreationAction, DummyTenantPostCreationActions>();
        }

        return services;
    }

    public static IApplicationBuilder SetupRbkDefaultAdmin(this IApplicationBuilder app, Action<rbkDefaultAdminOptions> configureOptions)
    {
        var options = new rbkDefaultAdminOptions();
        configureOptions(options);

        if (String.IsNullOrEmpty(options._username)) throw new ArgumentNullException("Username", "You must provide a name for the admin user");
        if (String.IsNullOrEmpty(options._password)) throw new ArgumentNullException("Password", "You must provide a password for the admin user");
        if (String.IsNullOrEmpty(options._email)) throw new ArgumentNullException("Email", "You must provide an email for the admin user");

        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

        using (var scope = scopeFactory.CreateScope())
        {
            var contexts = scope.ServiceProvider.GetServices<DbContext>().ToList();

            var context = contexts.GetDefaultContext();

            var user = context.Set<User>().FirstOrDefault(x => x.Username == options._username && String.IsNullOrEmpty(x.TenantId));

            if (user == null)
            {
                user = new User(null, options._username, options._email, options._password, options._avatar, options._displayName);
                user.Confirm();
                var manageTenantsClaim = context.Set<Claim>().FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_TENANTS);
                if (manageTenantsClaim == null) throw new NullReferenceException($"Could not find the {AuthenticationClaims.MANAGE_TENANTS} claim");

                var manageClaimsClaim = context.Set<Claim>().FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_CLAIMS);
                if (manageClaimsClaim == null) throw new NullReferenceException($"Could not find the {AuthenticationClaims.MANAGE_CLAIMS} claim");

                var manageClaimProtectionClaim = context.Set<Claim>().FirstOrDefault(x => x.Identification == AuthenticationClaims.CHANGE_CLAIM_PROTECTION);
                if (manageClaimProtectionClaim == null) throw new NullReferenceException($"Could not find the {AuthenticationClaims.CHANGE_CLAIM_PROTECTION} claim");

                var manageApplicationRolesClaim = context.Set<Claim>().FirstOrDefault(x => x.Identification == AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES);
                if (manageApplicationRolesClaim == null) throw new NullReferenceException($"Could not find the {AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES} claim");

                user.AddClaim(manageTenantsClaim, ClaimAccessType.Allow);
                user.AddClaim(manageClaimsClaim, ClaimAccessType.Allow);
                user.AddClaim(manageClaimProtectionClaim, ClaimAccessType.Allow);
                user.AddClaim(manageApplicationRolesClaim, ClaimAccessType.Allow);
                context.Add(user);
                context.SaveChanges();
            }
        }

        return app;
    }

    public static IApplicationBuilder SetupRbkAuthenticationClaims(this IApplicationBuilder app, Action<rbkDefaultClaimOptions> configureOptions = null)
    {
        var options = new rbkDefaultClaimOptions();

        if (configureOptions != null)
        {
            configureOptions(options);
        }

        var scopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();

        using (var scope = scopeFactory.CreateScope())
        {
            var contexts = scope.ServiceProvider.GetServices<DbContext>().ToList();

            var context = contexts.GetDefaultContext();

            var claims = context.Set<Claim>().ToList();

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageApplicationWideRoles)
                    ? options._claimDescriptions.ManageApplicationWideRoles
                    : AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_APPLICATION_WIDE_ROLES, description);
                newClaim.Hide();
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageTenantSpecificRoles)
                    ? options._claimDescriptions.ManageTenantSpecificRoles
                    : AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES, description);
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_USER_ROLES))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageUserRoles)
                    ? options._claimDescriptions.ManageUserRoles
                    : AuthenticationClaims.MANAGE_USER_ROLES;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_USER_ROLES, description);
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.OVERRIDE_USER_CLAIMS))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.OverrideUserClaims)
                    ? options._claimDescriptions.OverrideUserClaims
                    : AuthenticationClaims.OVERRIDE_USER_CLAIMS;

                var newClaim = new Claim(AuthenticationClaims.OVERRIDE_USER_CLAIMS, description);
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_CLAIMS))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageClaims)
                    ? options._claimDescriptions.ManageClaims
                    : AuthenticationClaims.MANAGE_CLAIMS;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_CLAIMS, description);
                newClaim.Hide();
                newClaim.Protect();

                context.Add(newClaim);
            }


            if (!claims.Any(x => x.Identification == AuthenticationClaims.CHANGE_CLAIM_PROTECTION))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ChangeClaimProtection)
                    ? options._claimDescriptions.ChangeClaimProtection
                    : AuthenticationClaims.CHANGE_CLAIM_PROTECTION;

                var newClaim = new Claim(AuthenticationClaims.CHANGE_CLAIM_PROTECTION, description);
                newClaim.Hide();
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_TENANTS))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageTenants)
                    ? options._claimDescriptions.ManageTenants
                    : AuthenticationClaims.MANAGE_TENANTS;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_TENANTS, description);
                newClaim.Hide();
                newClaim.Protect();

                context.Add(newClaim);
            }

            if (!claims.Any(x => x.Identification == AuthenticationClaims.MANAGE_USERS))
            {
                var description = options._claimDescriptions != null && !String.IsNullOrEmpty(options._claimDescriptions.ManageUsers)
                    ? options._claimDescriptions.ManageUsers
                    : AuthenticationClaims.MANAGE_USERS;

                var newClaim = new Claim(AuthenticationClaims.MANAGE_USERS, description);
                newClaim.Protect();

                context.Add(newClaim);
            }

            context.SaveChanges();

            context.Dispose();
        }

        return app;
    }
}