namespace rbkApiModules.Identity.Core;

public class UpdateClaim : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/authorization/claims", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.MANAGE_CLAIMS)
        .WithName("Update Claim")
        .WithTags("Claims");
    }

    public class Request : ICommand
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public bool AllowApiKeyUsage { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(x => x.Id)
                .ClaimExistOnDatabase(claimsService, localization);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MustAsync(ClaimAlreadyUsedInApiKeys)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimAlreadyUsedInApiKeys))
                .MustAsync(NotExistsInDatabaseWithSameDescription)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimDescriptionAlreadyUsed));
        }

        private async Task<bool> NotExistsInDatabaseWithSameDescription(Request request, string identification, CancellationToken cancellationToken)
        {
            var existing = await _claimsService.FindByDescriptionAsync(identification, cancellationToken);
            return existing == null || existing.Id == request.Id;
        }

        private async Task<bool> ClaimAlreadyUsedInApiKeys(Request request, string identification, CancellationToken cancellationToken)
        {
            if (!request.AllowApiKeyUsage)
            {
                var currentClaim = await _claimsService.FindAsync(request.Id, cancellationToken);
                if (currentClaim.AllowApiKeyUsage && await _claimsService.IsUsedByAnyApiKeysAsync(request.Id, cancellationToken))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
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
            await _claimsService.RenameAsync(request.Id, request.Description, cancellationToken);
            await _claimsService.SetAllowApiKeyUsageAsync(request.Id, request.AllowApiKeyUsage, cancellationToken);

            var claim = await _claimsService.FindAsync(request.Id, cancellationToken);

            return CommandResponse.Success(ClaimDetails.FromModel(claim));
        }
    }
}