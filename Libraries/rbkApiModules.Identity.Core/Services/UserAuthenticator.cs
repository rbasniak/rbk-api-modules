using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace rbkApiModules.Identity.Core;

public interface IUserAuthenticator
{
    Task<JwtResponse> Authenticate(string username, string tenant, JwtOptionsOverride jwtOptionsOverride, CancellationToken cancellation);
}

public class UserAuthenticator : IUserAuthenticator
{
    private readonly IAuthService _authService;
    private readonly IJwtFactory _jwtFactory;
    private readonly JwtIssuerOptions _jwtOptions;
    private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;
    private readonly IAutomaticUserCreator _automaticUserCreator;

    public UserAuthenticator(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IAuthService authService,
    IEnumerable<ICustomClaimHandler> claimHandlers, IAutomaticUserCreator automaticUserCreator)
    {
        _jwtFactory = jwtFactory;
        _authService = authService;
        _jwtOptions = jwtOptions.Value;
        _claimHandlers = claimHandlers;
        _automaticUserCreator = automaticUserCreator;
    }

    public async Task<JwtResponse> Authenticate(string username, string tenant, JwtOptionsOverride jwtOptionsOverride, CancellationToken cancellation)
    {
        var user = await _authService.FindUserAsync(username, tenant, cancellation);

        if (user == null)
        {
            user = await _automaticUserCreator.CreateIfAllowedAsync(username, tenant, cancellation);
        }

        Log.Information($"Loging in with user {user.Username}");

        if (user.RefreshTokenValidity < DateTime.UtcNow)
        {
            var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

            await _authService.UpdateRefreshTokenAsync(username, tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellation);
        }

        user = await _authService.GetUserWithDependenciesAsync(username, tenant, cancellation);

        var extraClaims = new Dictionary<string, string[]>
            {
                { JwtClaimIdentifiers.AuthenticationMode, new[] { user.AuthenticationMode.ToString() } }
            };

        Log.Information($"Token generated with AuthenticationMode={user.AuthenticationMode}");

        foreach (var claimHandler in _claimHandlers)
        {
            foreach (var claim in await claimHandler.GetClaims(user.TenantId, user.Username))
            {
                extraClaims.Add(claim.Type, new[] { claim.Value });
            }
        }

        var jwt = await TokenGenerator.GenerateAsync(_jwtFactory, user, extraClaims, jwtOptionsOverride);

        await _authService.RefreshLastLogin(username, tenant,  cancellation);

        return jwt;
    }
}

public class JwtOptionsOverride
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? ExpiresAt { get; set; } 
}
