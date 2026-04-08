using Microsoft.AspNetCore.Authentication;

namespace rbkApiModules.Identity.Core;

internal sealed class RbkApiKeyAuthorizationFilter : IEndpointFilter
{
    private readonly string _requiredClaim;

    internal RbkApiKeyAuthorizationFilter(string requiredClaim)
    {
        _requiredClaim = requiredClaim;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            var authResult = await context.HttpContext.AuthenticateAsync(RbkAuthenticationSchemes.API_KEY);
            if (authResult.Succeeded)
            {
                context.HttpContext.User = authResult.Principal;
                user = authResult.Principal;
            }
        }

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return Results.Unauthorized();
        }

        if (user.Identity?.AuthenticationType != RbkAuthenticationSchemes.API_KEY)
        {
            return Results.Forbid();
        }

        var roles = user.Claims
            .Where(x => x.Type == JwtClaimIdentifiers.Roles)
            .Select(x => x.Value)
            .ToList();

        if (!roles.Contains(_requiredClaim))
        {
            return Results.Forbid();
        }

        return await next(context);
    }
}
