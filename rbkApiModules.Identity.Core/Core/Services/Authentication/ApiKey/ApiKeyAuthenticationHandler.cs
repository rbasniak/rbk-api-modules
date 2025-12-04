using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace rbkApiModules.Identity.Core;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    protected string _providedApiKey;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IApiKeyValidator apiKeyValidator)
        : base(options, logger, encoder)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Get the API key from the request headers
        if (!Request.Headers.TryGetValue(RbkAuthenticationSchemes.API_KEY, out var apiKeyHeaderValues))
        {
            return AuthenticateResult.Fail("API Key is missing");
        }

        _providedApiKey = apiKeyHeaderValues.FirstOrDefault();

        // Validate the API key (you can replace this with your own logic)
        if (await _apiKeyValidator.ValidateApiKey(_providedApiKey))
        {
            var identity = GetClaimsIdentity();

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
        else
        {
            return AuthenticateResult.Fail("Invalid API Key");
        }
    }

    protected virtual ClaimsIdentity GetClaimsIdentity()
    {
        var claims = new[] { new System.Security.Claims.Claim(RbkAuthenticationSchemes.API_KEY, _providedApiKey) };

        return new ClaimsIdentity(claims, GetIdentityName());
    }

    protected virtual string GetIdentityName()
    {
        return Scheme.Name;
    }
}

public interface IApiKeyValidator
{
    Task<bool> ValidateApiKey(string apiKey);
}