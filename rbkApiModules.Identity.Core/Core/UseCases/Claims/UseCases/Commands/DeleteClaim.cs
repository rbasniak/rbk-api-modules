namespace rbkApiModules.Identity.Core;

public class DeleteClaim : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/authorization/claims/{id}", async (Guid id, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(new Request { Id = id }, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization(AuthenticationClaims.MANAGE_CLAIMS)
        .WithName("Delete Claim")
        .WithTags("Claims");
    }

    public class Request : ICommand
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(x => x.Id)
                .ClaimExistOnDatabase(claimsService, localization)
                .MustAsync(NotBeUsedInAnyRole)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.CannotRemoveClaimUsedByOtherRoles))
                .MustAsync(NotBeProtected)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.CannotRemoveSystemProtectedClaims))
                .MustAsync(NotBeUsedInAnyUser)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.CannotRemoveClaimAssociatedWithUsers));
        }

        private async Task<bool> NotBeUsedInAnyRole(Request request, Guid id, CancellationToken cancellationToken)
        {
            return !await _claimsService.IsUsedByAnyRolesAsync(id, cancellationToken);
        }

        private async Task<bool> NotBeUsedInAnyUser(Request request, Guid id, CancellationToken cancellationToken)
        {
            return !await _claimsService.IsUsedByAnyUsersAsync(id, cancellationToken);
        }
        private async Task<bool> NotBeProtected(Request request, Guid id, CancellationToken cancellationToken)
        {
            var claim = await _claimsService.FindAsync(id, cancellationToken);

            return !claim.IsProtected;
        }
    }
    public class Handler(IClaimsService _claimsService) : ICommandHandler<Request>
    {

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            await _claimsService.DeleteAsync(request.Id, cancellationToken);

            return CommandResponse.Success();
        }
    }
}