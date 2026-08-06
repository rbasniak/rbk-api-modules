using Demo2.Clients;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Demo2.Endpoints;

public static class IntegrationEndpoints
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/integration/demo1/anonymous", async (IDemo1ApiClient demo1ApiClient, CancellationToken cancellationToken) =>
        {
            var message = await demo1ApiClient.GetAnonymousMessageAsync(cancellationToken);

            return Results.Ok(new
            {
                source = "Demo2",
                sibling = "Demo1",
                message
            });
        })
        .AllowAnonymous()
        .WithName("Demo2 calls Demo1 anonymous endpoint")
        .WithTags("Integration");
    }
}
