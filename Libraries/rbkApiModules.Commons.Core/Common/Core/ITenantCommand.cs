using System.Security.Claims;
using System.Text.Json.Serialization;

namespace rbkApiModules.Commons.Core;

public interface IAuthenticatedRequest
{
    AuthenticatedUser Identity { get; }
    bool IsAuthenticated { get; }

    void SetIdentity(string tenant, string username, string[] claims);
}

public abstract class AuthenticatedRequest: IAuthenticatedRequest
{
    public AuthenticatedRequest()
    {
        Identity = AuthenticatedUser.Empty();
        IsAuthenticated = false;
    }

    [JsonIgnore]
    public AuthenticatedUser Identity { get; private set; }

    [JsonIgnore]
    public bool IsAuthenticated { get; private set; }

    public void SetIdentity(string tenant, string username, string[] claims)
    {
        Identity = new AuthenticatedUser(tenant, username, claims);
        IsAuthenticated = true;
    } 
}

public class AuthenticatedUser
{
    private readonly string[] _claims;

    public AuthenticatedUser(string tenant, string username, string[] claims)
    {
        Tenant = tenant;
        Username = username;
        _claims = claims;
    }

    public bool IsAuthenticated => !String.IsNullOrEmpty(Username.Trim());
    public bool HasTenant => !String.IsNullOrEmpty(Tenant?.Trim());
    public bool HasNoTenant => String.IsNullOrEmpty(Tenant?.Trim());
    public IEnumerable<string> Claims => _claims;
    public string Username { get; }
    public string Tenant { get; }

    public bool HasClaim(string claim)
    {
        return _claims.Any(x => x.ToLower() == claim.ToLower());
    }

    public static AuthenticatedUser Empty()
    {
        return new AuthenticatedUser(null, String.Empty, new string[0]);
    }
}