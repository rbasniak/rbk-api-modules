using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Core;

public class RevokeApiKey : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/api-keys/revoke", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);
            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.CAN_MANAGE_APIKEYS)
        .WithName("Revoke API Key")
        .WithTags("ApiKeys");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public Guid Id { get; set; }

        public required string Reason { get; set; }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly DbContext _context;
        private readonly IApiKeyAuthenticationCacheInvalidation _cacheInvalidation;

        public Handler(IEnumerable<DbContext> contexts, IApiKeyAuthenticationCacheInvalidation cacheInvalidation)
        {
            _context = contexts.GetDefaultContext();
            _cacheInvalidation = cacheInvalidation;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            if (!request.IsAuthenticated)
            {
                return CommandResponse.Forbidden("Authentication is required.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
            {
                return CommandResponse.Failure("Revoke reason is required.");
            }

            var reason = request.Reason.Trim();

            if (reason.Length > 2048)
            {
                return CommandResponse.Failure("Revoke reason must be at most 2048 characters.");
            }

            var entity = await _context.Set<ApiKey>()
                .Include(x => x.Claims)
                .ThenInclude(x => x.Claim)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return CommandResponse.Failure("API key not found.");
            }

            if (!ApiKeyAuthorization.CanManageExistingKeyInScope(request.Identity, entity.TenantId))
            {
                return CommandResponse.Forbidden("You are not allowed to manage this API key.");
            }

            if (!entity.IsActive)
            {
                return CommandResponse.Failure("API key is already revoked.");
            }

            var hash = entity.KeyHash;
            entity.Revoke(reason);
            await _context.SaveChangesAsync(cancellationToken);

            _cacheInvalidation.InvalidateByKeyHash(hash);

            return CommandResponse.Success(ApiKeyDetails.FromModel(entity));
        }
    }
}
