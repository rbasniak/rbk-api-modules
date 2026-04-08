using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Core;

public class ListApiKeys : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/authorization/api-keys", async (IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(new Request(), cancellationToken);
            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.CAN_MANAGE_APIKEYS)
        .WithName("List API Keys")
        .WithTags("ApiKeys");
    }

    public class Request : IQuery
    {
    }

    public class Handler : IQueryHandler<Request>
    {
        private readonly DbContext _context;

        public Handler(IEnumerable<DbContext> contexts)
        {
            _context = contexts.GetDefaultContext();
        }

        public async Task<QueryResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var keys = await _context.Set<ApiKey>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
                .ToArrayAsync(cancellationToken);

            var dtos = keys.Select(ApiKeyDetails.FromModel).ToList();
            return QueryResponse.Success(dtos);
        }
    }
}
