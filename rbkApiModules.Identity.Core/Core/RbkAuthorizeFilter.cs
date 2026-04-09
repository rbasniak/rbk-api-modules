using Microsoft.AspNetCore.Authentication;

namespace rbkApiModules.Identity.Core;

internal sealed class RbkAuthorizationFilter : IEndpointFilter
{
    private readonly string _requiredClaim;

    internal RbkAuthorizationFilter(string requiredClaim)
    {
        _requiredClaim = requiredClaim;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var user = context.HttpContext.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            var apiKeyAuth = await context.HttpContext.AuthenticateAsync(RbkAuthenticationSchemes.API_KEY);
            if (apiKeyAuth.Succeeded)
            {
                context.HttpContext.User = apiKeyAuth.Principal;
                user = apiKeyAuth.Principal;
            }
        }

        if (!user.Identity?.IsAuthenticated ?? false)
        {
            return Results.Unauthorized();
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
