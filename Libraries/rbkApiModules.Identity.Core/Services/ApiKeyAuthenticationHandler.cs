using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace rbkApiModules.Identity.Core;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly IApiKeyValidator _apiKeyValidator;

    protected string[] _providedApiKeys;
    protected string _matchedApiKey;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, IApiKeyValidator apiKeyValidator)
        : base(options, logger, encoder)
    {
        _apiKeyValidator = apiKeyValidator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var foundApiKeys = new List<string>();

        foreach(var validHeader in RbkAuthenticationSchemes.ValidApiKeyHeaders)
        {
            if (Request.Headers.TryGetValue(validHeader, out var apiKeyHeaderValues))
            {
                var value = apiKeyHeaderValues.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    foundApiKeys.Add(value);
                }
            }
        }

        foundApiKeys = foundApiKeys
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.Ordinal)
            .ToList();

        _providedApiKeys = foundApiKeys.ToArray();

        if (!foundApiKeys.Any())
        {
            return AuthenticateResult.Fail("API Key is missing");
        }

        foreach (var apiKey in foundApiKeys)
        {
            // Validate the API key (you can replace this with your own logic)
            if (await _apiKeyValidator.ValidateApiKey(apiKey))
            {
                _matchedApiKey = apiKey;

                var identity = GetClaimsIdentity();

                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
        }
        
        return AuthenticateResult.Fail("Invalid API Key");
    }

    protected virtual ClaimsIdentity GetClaimsIdentity()
    {
        var claims = new[] { new System.Security.Claims.Claim(RbkAuthenticationSchemes.API_KEY, _matchedApiKey) };

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