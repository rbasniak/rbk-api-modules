using System.Linq.Expressions;
using System.Reflection;

namespace rbkApiModules.Identity.Core;

public class RbkAuthenticationOptions
{
    // TODO: export internals para as libs corretas
    public bool _disablePasswordReset = false;
    public bool _disableEmailConfirmation = false;
    public bool _disableRefreshToken = false;
    public bool _disablePasswordAuthentication = false;
    public bool _useAssymetricEncryptationKey = false;
    public bool _useSymetricEncryptationKey = false;
    public Type _tenantPostCreationActionType = null;
    public NtlmMode _ntlmMode = NtlmMode.None;

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

    public RbkAuthenticationOptions DisablePasswordAuthentication()
    {
        _disablePasswordAuthentication = true;

        return this;
    }

    public RbkAuthenticationOptions EnableWindowsAuthentication(NtlmMode mode)
    {
        _ntlmMode = mode;

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

public class AuthEmailOptions
{
    public ServerOptions Server { get; set; }
    public SenderOptions Sender { get; set; }
    public EmailContentOptions EmailData { get; set; }
    public TestOptions TestMode { get; set; }
}

public class TestOptions
{
    public bool Enabled { get; set; }
    public string OutputFolder { get; set; }
}

public class ServerOptions
{
    public string SmtpHost { get; set; }
    public int Port { get; set; }
    public bool SSL { get; set; }
}

public class SenderOptions
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class EmailContentOptions
{
    public string MainColor { get; set; }
    public string FontColor { get; set; }
    public string Logo { get; set; }
    public string SuportEmail { get; set; }
    public string AccountDetailsUrl { get; set; }
    public string PasswordResetUrl { get; set; }
    public string ConfirmationSuccessUrl { get; set; }
    public string ConfirmationFailedUrl { get; set; }
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

