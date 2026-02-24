using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;
using System.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection;

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

        services.AddSingleton(new RbkDefaultAdminOptions());

        return services;
    }

    public static IApplicationBuilder UseRbkRelationalAuthentication(this WebApplication app)
    {
        var authenticationOptions = app.Services.GetRequiredService<RbkAuthenticationOptions>();

        var endpoints = new List<(string Key, Action<IEndpointRouteBuilder> Map)>
        {
            ("RedefinePassword", RedefinePassword.MapEndpoint),
            ("RequestPasswordReset", RequestPasswordReset.MapEndpoint),
            ("ConfirmUserEmail", ConfirmUserEmail.MapEndpoint),
            ("ResendEmailConfirmation", ResendEmailConfirmation.MapEndpoint),
            ("RenewAccessToken", RenewAccessToken.MapEndpoint),
            ("GetAllTenants.Authenticated", GetAllTenants.MapEndpointAuthenticated),
            ("GetAllTenants.Anonymous", GetAllTenants.MapEndpointAnonymous),
            ("UserLogin.Credentials", UserLogin.MapCredentialsLoginEndpoint),
            ("UserLogin.Ntlm", UserLogin.MapNtlmLoginEndpoint),
            ("ChangePassword", ChangePassword.MapEndpoint),
            ("SwitchTenant", SwitchTenant.MapEndpoint),
            ("CreateUser", CreateUser.MapEndpoint),
            ("Register", Register.MapEndpoint),
            ("ActivateUser", ActivateUser.MapEndpoint),
            ("DeactivateUser", DeativateUser.MapEndpoint),
            ("DeleteUser", DeleteUser.MapEndpoint),
            ("GetAllUsers", GetAllUsers.MapEndpoint),
            ("ReplaceUserRoles", ReplaceUserRoles.MapEndpoint),
            ("AddClaimOverride", AddClaimOverride.MapEndpoint),
            ("CreateTenant", CreateTenant.MapEndpoint),
            ("DeleteTenant", DeleteTenant.MapEndpoint),
            ("UpdateTenant", UpdateTenant.MapEndpoint),
            ("GetAllRoles", GetAllRoles.MapEndpoint),
            ("CreateRole", CreateRole.MapEndpoint),
            ("DeleteRole", DeleteRole.MapEndpoint),
            ("RenameRole", RenameRole.MapEndpoint),
            ("UpdateRoleClaims", UpdateRoleClaims.MapEndpoint),
            ("GetAllClaims", GetAllClaims.MapEndpoint),
            ("CreateClaim", CreateClaim.MapEndpoint),
            ("DeleteClaim", DeleteClaim.MapEndpoint),
            ("ProtectClaim", ProtectClaim.MapEndpoint),
            ("UnprotectClaim", UnprotectClaim.MapEndpoint),
            ("UpdateClaim", UpdateClaim.MapEndpoint),
        };

        if (authenticationOptions._disablePasswordReset)
        {
            endpoints.RemoveAll(x => x.Key is "RedefinePassword" or "RequestPasswordReset");
        }

        if (authenticationOptions._disableEmailConfirmation)
        {
            endpoints.RemoveAll(x => x.Key is "ConfirmUserEmail" or "ResendEmailConfirmation");
        }

        if (authenticationOptions._disableRefreshToken)
        {
            endpoints.RemoveAll(x => x.Key == "RenewAccessToken");
        }

        if (authenticationOptions._allowAnonymousTenantAccess)
        {
            endpoints.RemoveAll(x => x.Key == "GetAllTenants.Authenticated");  
        }
        else
        {
            endpoints.RemoveAll(x => x.Key == "GetAllTenants.Anonymous");  
        }

        if (authenticationOptions._loginMode == LoginMode.WindowsAuthentication || authenticationOptions._loginMode == LoginMode.Custom)
        {
            endpoints.RemoveAll(e =>
                e.Key is "UserLogin.Credentials"
                or "ChangePassword"
                or "ConfirmUserEmail"
                or "ResendEmailConfirmation"
                or "RedefinePassword"
                or "RequestPasswordReset");

            app.UseMiddleware<WindowsAuthenticationMiddleware>();
        }

        if (authenticationOptions._loginMode == LoginMode.Credentials || authenticationOptions._loginMode == LoginMode.Custom)
        {
            endpoints.RemoveAll(x => x.Key == "UserLogin.Ntlm");
        }

        if (!authenticationOptions._allowTenantSwitching)
        {
            endpoints.RemoveAll(x => x.Key == "SwitchTenant");
        }

        if (!authenticationOptions._allowUserSelfRegistration)
        {
            endpoints.RemoveAll(x => x.Key == "Register");
        }
        else if (authenticationOptions._loginMode is LoginMode.WindowsAuthentication or LoginMode.Custom)
        {
            throw new NotSupportedException("User self registration is not allowed with Windows authentication");
        }

        if (!authenticationOptions._allowUserCreationByAdmin)
        {
            endpoints.RemoveAll(x => x.Key == "CreateUser");
        }

        foreach (var (_, map) in endpoints)
        {
            map(app);
        }

        return app;
    }

    public static IApplicationBuilder SetupRbkDefaultAdmin(this IApplicationBuilder app, Action<RbkDefaultAdminOptions> configureOptions)
    {
        var options = app.ApplicationServices.GetService<RbkDefaultAdminOptions>();
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
                user = new User(null, options._username, options._email, options._password, options._avatar, options._displayName, AuthenticationMode.Credentials);
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

            Debug.WriteLine(context.Database.GetDbConnection().ConnectionString);

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