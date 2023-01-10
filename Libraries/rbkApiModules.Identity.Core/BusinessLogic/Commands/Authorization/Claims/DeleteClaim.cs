using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class DeleteClaim
{
    public class Command : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(a => a.Id)
                .ClaimExistOnDatabase(claimsService, localization)
                .MustAsync(NotBeUsedInAnyRole).WithMessage(localization.GetValue("Cannot remove a claim that is being used by any roles"))
                .MustAsync(NotBeProtected).WithMessage(localization.GetValue("Cannot remove a system protected claim"))
                .MustAsync(NotBeUsedInAnyUser).WithMessage(localization.GetValue("Cannot remove a claim that is being used in any users"));
        } 

        private async Task<bool> NotBeUsedInAnyRole(Command command, Guid id, CancellationToken cancellation)
        {
            return !await _claimsService.IsUsedByAnyRolesAsync(id, cancellation);
        }

        private async Task<bool> NotBeUsedInAnyUser(Command command, Guid id, CancellationToken cancellation)
        {
            return !await _claimsService.IsUsedByAnyUsersAsync(id, cancellation);
        }
        private async Task<bool> NotBeProtected(Command command, Guid id, CancellationToken cancellation)
        {
            var claim = await _claimsService.FindAsync(id, cancellation);

            return !claim.IsProtected;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _claimsService.DeleteAsync(request.Id, cancellation);

            return CommandResponse.Success();
        }
    }
}