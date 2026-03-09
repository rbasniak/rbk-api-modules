namespace rbkApiModules.Identity.Core;


public class CreateClaim : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/claims", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.MANAGE_CLAIMS)
        .WithName("Create Claim")
        .WithTags("Claims");
    }

    public class Request : ICommand
    {
        public required string Identification { get; set; }
        public required string Description { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(x => x.Identification)
                .NotEmpty()
                .MustAsync(NotExistsInDatabaseWithSameIdentification)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimIdentificationAlreadyUsed));

            RuleFor(x => x.Description)
                .NotEmpty()
                .MustAsync(NotExistsInDatabaseWithSameDescription)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimDescriptionAlreadyUsed));
        }

        private async Task<bool> NotExistsInDatabaseWithSameIdentification(Request request, string identification, CancellationToken cancellationToken)
        {
            return (await _claimsService.FindByIdentificationAsync(identification, cancellationToken)) == null;
        }

        private async Task<bool> NotExistsInDatabaseWithSameDescription(Request request, string identification, CancellationToken cancellationToken)
        {
            return (await _claimsService.FindByDescriptionAsync(identification, cancellationToken)) == null;
        }
    }

    public class Handler(IClaimsService _claimsService) : ICommandHandler<Request>
    {

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var claim = new Claim(request.Identification, request.Description);

            claim = await _claimsService.CreateAsync(claim, cancellationToken);

            return CommandResponse.Success(ClaimDetails.FromModel(claim));
        }
    }
}