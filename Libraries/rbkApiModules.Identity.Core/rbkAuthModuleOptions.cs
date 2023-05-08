using System.Linq.Expressions;
using System.Reflection;

namespace rbkApiModules.Identity.Core;

public class RbkAuthenticationOptions
{
    internal bool _disablePasswordReset = false;
    internal bool _disableEmailConfirmation = false;
    internal bool _disableRefreshToken = false;
    internal bool _useAssymetricEncryptationKey = false;
    internal bool _useSymetricEncryptationKey = false;
    internal bool _allowAnonymousTenantAccess = false;
    internal Type _tenantPostCreationActionType = null;
    internal LoginMode _loginMode = LoginMode.Credentials;
    internal bool _allowUserSelfRegistration = false;
    internal bool _allowTenantSwitching = false;
    internal bool _allowUserCreationByAdmin = false;
    internal string _userAvatarPath = "users/images";
    internal Type _customAvatarStorageType = typeof(DefaultAvatarStorageService);
    internal bool _useMockedWindowsAuthentication = false;

    public RbkAuthenticationOptions UseCustomAvatarStorage<T>() where T : IAvatarStorage
    {
        _customAvatarStorageType = typeof(T);

        return this;
    }

    public RbkAuthenticationOptions SetDefaultAvatarStoragePath(string path)
    {
        _userAvatarPath = path;

        return this;
    }

    public RbkAuthenticationOptions DisablePasswordReset()
    {
        _disablePasswordReset = true;

        return this;
    }

    public RbkAuthenticationOptions DisableEmailConfirmation()
    {
        _disableEmailConfirmation = true;

        return this;
    }

    public RbkAuthenticationOptions DisableRefreshToken()
    {
        _disableRefreshToken = true;

        return this;
    }

    public RbkAuthenticationOptions UseLoginWithWindowsAuthentication()
    {
        _loginMode = LoginMode.WindowsAuthentication;

        return this;
    }

    public RbkAuthenticationOptions UseAssymetricEncryptationKey()
    {
        _useAssymetricEncryptationKey = true;

        return this;
    }

    public RbkAuthenticationOptions UseSymetricEncryptationKey()
    {
        _useSymetricEncryptationKey = true;

        return this;
    }

    public RbkAuthenticationOptions UseTenantPostCreationAction(Type type)
    {
        _tenantPostCreationActionType = type;

        return this;
    }

    public RbkAuthenticationOptions AllowAnonymousAccessToTenants()
    {
        _allowAnonymousTenantAccess = true;

        return this;
    }

    public RbkAuthenticationOptions AllowUserSelfRegistration()
    {
        _allowUserSelfRegistration = true;

        return this;
    }

    public RbkAuthenticationOptions AllowTenantSwitching()
    {
        _allowTenantSwitching = true;

        return this;
    }

    public RbkAuthenticationOptions AllowUserCreationByAdmins()
    {
        _allowUserCreationByAdmin = true;

        return this;
    }
    
    public RbkAuthenticationOptions UseMockedWindowsAuthentication()
    {
        _useMockedWindowsAuthentication = true;

        return this;
    }
}

public class rbkDefaultClaimOptions
{
    // TODO: Export internals para a lib correta 
    public SeedClaimDescriptions _claimDescriptions = new();

    public rbkDefaultClaimOptions()
    {
        _claimDescriptions.ManageTenantSpecificRoles = "[Security] Manage tenant specific roles";
        _claimDescriptions.ManageApplicationWideRoles = "[Security] Manage application wide roles";
        _claimDescriptions.ManageClaims = "[Security] Manage claims";
        _claimDescriptions.ManageTenants = "[Security] Manage tenants";
        _claimDescriptions.ManageUserRoles = "[Security] Manage user roles";
        _claimDescriptions.OverrideUserClaims = "[Security] Override user claims individually";
        _claimDescriptions.ChangeClaimProtection = "[Security] Change claim protection";
        _claimDescriptions.ManageUsers = "[Security] Manage users";
    }

    public rbkDefaultClaimOptions WithCustomDescription(Expression<Func<SeedClaimDescriptions, string>> memberLamda, string description)
    {
        var memberSelectorExpression = memberLamda.Body as MemberExpression;
        if (memberSelectorExpression != null)
        {
            var property = memberSelectorExpression.Member as PropertyInfo;
            
            if (property != null)
            {
                property.SetValue(_claimDescriptions, description, null);
            }
        }

        return this;
    } 

    public rbkDefaultClaimOptions WithCustomDescriptions(SeedClaimDescriptions descriptions)
    {
        _claimDescriptions = descriptions;

        return this;
    } 
}

public class rbkDefaultAdminOptions
{
    // TODO: Expor internal para core only
    public bool _manageTenants = false;
    public string _password = null;
    public string _username = null;
    public string _email = null;
    public string _displayName = null;
    public string _avatar = null;

    public rbkDefaultAdminOptions()
    {
    }

    public rbkDefaultAdminOptions AllowTenantManagement()
    {
        _manageTenants = true;

        return this;
    }
    public rbkDefaultAdminOptions WithUsername(string username)
    {
        _username = username;

        return this;
    }
    public rbkDefaultAdminOptions WithPassword(string password)
    {
        _password = password;

        return this;
    }

    public rbkDefaultAdminOptions WithEmail(string email)
    {
        _email = email;

        return this;
    }

    public rbkDefaultAdminOptions WithDisplayName(string displayName)
    {
        _displayName = displayName;

        return this;
    }

    public rbkDefaultAdminOptions WithAvatar(string avatar)
    {
        _avatar = avatar;

        return this;
    }
}

public class SeedClaimDescriptions
{
    public string OverrideUserClaims { get; set; }
    public string ManageUserRoles { get; set; }
    public string ManageTenantSpecificRoles { get; set; }
    public string ManageApplicationWideRoles { get; set; }
    public string ManageClaims { get; set; }
    public string ChangeClaimProtection { get; set; }
    public string ManageTenants { get; set; }
    public string ManageUsers { get; set; }
}

