using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Core;

public class UpdateApiKey : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/authorization/api-keys", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);
            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.CAN_MANAGE_APIKEYS)
        .WithName("Update API Key")
        .WithTags("ApiKeys");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public bool IsActive { get; set; }

        public required IReadOnlyList<Guid> ClaimIds { get; set; }
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

            if (request.ClaimIds == null || request.ClaimIds.Count == 0)
            {
                return CommandResponse.Failure("At least one claim is required.");
            }

            if (request.ClaimIds.Distinct().Count() != request.ClaimIds.Count)
            {
                return CommandResponse.Failure("Duplicate claim identifiers in the request.");
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

            var previousHash = entity.KeyHash;

            var claimIds = request.ClaimIds.ToArray();
            var claims = await _context.Set<Claim>().Where(x => claimIds.Contains(x.Id)).ToArrayAsync(cancellationToken);

            if (claims.Length != claimIds.Length)
            {
                return CommandResponse.Failure("One or more claims were not found.");
            }

            foreach (var claim in claims)
            {
                if (!claim.AllowApiKeyUsage)
                {
                    return CommandResponse.Failure($"Claim '{claim.Identification}' is not allowed on API keys.");
                }
            }

            entity.SetName(request.Name);
            entity.SetExpirationDate(request.ExpirationDate);
            entity.SetIsActive(request.IsActive);

            entity.ReplaceClaims(claims);

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(entity).Collection(x => x.Claims).Query().Include(x => x.Claim).LoadAsync(cancellationToken);

            _cacheInvalidation.InvalidateByKeyHash(previousHash);

            return CommandResponse.Success(ApiKeyDetails.FromModel(entity));
        }
    }
}
