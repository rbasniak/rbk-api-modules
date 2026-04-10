namespace rbkApiModules.Identity.Core;

public class UnprotectClaim : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/claims/unprotect", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.CHANGE_CLAIM_PROTECTION)
        .WithName("Unprotect Claim")
        .WithTags("Claims");
    }

    public class Request : ICommand
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            RuleFor(x => x.Id)
                .ClaimExistOnDatabase(claimsService, localization);
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            await _claimsService.UnprotectAsync(request.Id, cancellationToken);

            var claim = await _claimsService.FindAsync(request.Id, cancellationToken);

            return CommandResponse.Success(ClaimDetails.FromModel(claim));
        }
    }
}