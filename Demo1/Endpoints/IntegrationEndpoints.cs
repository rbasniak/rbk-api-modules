using Demo1.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Demo1.Endpoints;

public static class IntegrationEndpoints
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/integration/demo2/anonymous", async (IDemo2ApiClient demo2ApiClient, CancellationToken cancellationToken) =>
        {
            var message = await demo2ApiClient.GetAnonymousMessageAsync(cancellationToken);

            return Results.Ok(new
            {
                source = "Demo1",
                sibling = "Demo2",
                message
            });
        })
        .AllowAnonymous()
        .WithName("Demo1 calls Demo2 anonymous endpoint")
        .WithTags("Integration");
    }
}
