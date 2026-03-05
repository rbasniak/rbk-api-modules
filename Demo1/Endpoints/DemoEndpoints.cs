using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using rbkApiModules.Identity.Core;

namespace Demo1.Endpoints;

public static class DemoEndpoints
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/demo/apikey", () => Results.Ok(new { message = "API key accepted" }))
            .RequireAuthorization(RbkAuthenticationSchemes.API_KEY_POLICY)
            .WithName("Demo API Key")
            .WithTags("Demo");

        endpoints.MapGet("/demo/jwt", () => Results.Ok(new { message = "JWT accepted" }))
            .RequireAuthorization()
            .WithName("Demo JWT")
            .WithTags("Demo");

        endpoints.MapGet("/demo/role", () => Results.Ok(new { message = "Role check passed" }))
            .RequireAuthorization()
            .RequireAuthorizationClaim(AuthenticationClaims.MANAGE_USERS)
            .WithName("Demo Role")
            .WithTags("Demo");

        endpoints.MapGet("/demo/anonymous", () => Results.Ok(new { message = "Anonymous" }))
            .AllowAnonymous()
            .WithName("Demo Anonymous")
            .WithTags("Demo");
    }
}
