namespace rbkApiModules.Identity.Core;

public class GetAllClaims : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/authorization/claims", async (IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(new Request(), cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .AllowAnonymous()
        .WithName("Get All Claims")
        .WithTags("Authorization");
    }

    public class Request : IQuery
    {
    }

    public class Handler(IClaimsService _claimsService) : IQueryHandler<Request>
    {

        public async Task<QueryResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var claim = await _claimsService.GetAllAsync(cancellationToken);

            return QueryResponse.Success(claim.Select(ClaimDetails.FromModel).AsReadOnly());
        }
    }
}
