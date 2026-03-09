using Microsoft.AspNetCore.Authorization;

namespace rbkApiModules.Identity.Core;

public class WindowsAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public WindowsAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var endpoint = context.GetEndpoint();
        var metadata = endpoint?.Metadata;

        var isAnonymous = metadata?.GetMetadata<AllowAnonymousAttribute>() != null;

        var authorizeAttr = metadata?.GetMetadata<AuthorizeAttribute>();
        var requiresApiKey = authorizeAttr?.AuthenticationSchemes == RbkAuthenticationSchemes.API_KEY
            || authorizeAttr?.Policy == RbkAuthenticationSchemes.API_KEY_POLICY;

        var hasBearerToken = context.Request.Headers.Authorization.ToString().ToLower().StartsWith("bearer ");

        if (context.Request.Path != "/api/authentication/login" &&
            !isAnonymous &&
            !hasBearerToken &&
            !requiresApiKey)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }
}
