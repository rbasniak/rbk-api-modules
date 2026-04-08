using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;

namespace rbkApiModules.Identity.Core;

public class CreateApiKey : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/api-keys", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);
            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.CAN_MANAGE_APIKEYS)
        .WithName("Create API Key")
        .WithTags("ApiKeys");
    }

    public class Request : ICommand
    {
        public required string Name { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public string? TenantId { get; set; }

        public required IReadOnlyList<Guid> ClaimIds { get; set; }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly DbContext _context;

        public Handler(IEnumerable<DbContext> contexts)
        {
            _context = contexts.GetDefaultContext();
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            if (request.ClaimIds == null || request.ClaimIds.Count == 0)
            {
                return CommandResponse.Failure("At least one claim is required.");
            }

            if (request.ClaimIds.Distinct().Count() != request.ClaimIds.Count)
            {
                return CommandResponse.Failure("Duplicate claim identifiers in the request.");
            }

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

            var (rawKey, keyPrefix, keyHash) = ApiKeyMaterial.Generate();

            var apiKey = new ApiKey(request.Name, keyHash, keyPrefix, request.TenantId, request.ExpirationDate);

            foreach (var claim in claims)
            {
                apiKey.AddClaim(claim);
            }

            await _context.AddAsync(apiKey, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            await _context.Entry(apiKey).Collection(x => x.Claims).Query().Include(x => x.Claim).LoadAsync(cancellationToken);

            var created = new Result
            {
                Id = apiKey.Id,
                RawKey = rawKey,
                KeyPrefix = keyPrefix,
                Name = apiKey.Name,
                ExpirationDate = apiKey.ExpirationDate,
                TenantId = apiKey.TenantId,
                AssignedClaims = apiKey.GetAccessClaims().Select(ClaimDetails.FromModel).ToList()
            };

            return CommandResponse.Success(created);
        }

        public sealed class Result
        {
            public required Guid Id { get; init; }

            public required string RawKey { get; init; }

            public required string KeyPrefix { get; init; }

            public required string Name { get; init; }

            public DateTime? ExpirationDate { get; init; }

            public string? TenantId { get; init; }

            public required IReadOnlyList<ClaimDetails> AssignedClaims { get; init; }
        }
    }
}
