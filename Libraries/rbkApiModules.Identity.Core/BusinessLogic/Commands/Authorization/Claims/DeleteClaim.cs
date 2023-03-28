using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class DeleteClaim
{
    public class Request : IRequest<CommandResponse>
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

        private async Task<bool> NotBeUsedInAnyRole(Request request, Guid id, CancellationToken cancellation)
        {
            return !await _claimsService.IsUsedByAnyRolesAsync(id, cancellation);
        }

        private async Task<bool> NotBeUsedInAnyUser(Request request, Guid id, CancellationToken cancellation)
        {
            return !await _claimsService.IsUsedByAnyUsersAsync(id, cancellation);
        }
        private async Task<bool> NotBeProtected(Request request, Guid id, CancellationToken cancellation)
        {
            var claim = await _claimsService.FindAsync(id, cancellation);

            return !claim.IsProtected;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _claimsService.DeleteAsync(request.Id, cancellation);

            return CommandResponse.Success();
        }
    }
}