using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class AddClaimOverride
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
        public Guid ClaimId { get; set; }
        public ClaimAccessType AccessType { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, IClaimsService claimsService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(a => a.Username)
                .IsRequired(localization)
                .MustAsync(UserExistInDatabaseUnderTheSameTenant).WithMessage(localization.GetValue("User not found."))
                .WithName(localization.GetValue("User"))
                .DependentRules(() =>
                {
                    RuleFor(a => a.ClaimId)
                        .ClaimExistOnDatabase(claimsService, localization);
                });
        } 

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Command command, string username, CancellationToken cancelation)
        {
            return await _authService.FindUserAsync(username, command.Identity.Tenant, cancelation) != null;
        } 
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IAuthService _authService;
        private readonly IClaimsService _claimsService;

        public Handler(IAuthService authService, IClaimsService claimsService)
        {
            _authService = authService;
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _claimsService.AddClaimOverrideAsync(request.ClaimId, request.Username, request.Identity.Tenant, request.AccessType, cancellation);

            var user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Identity.Tenant, cancellation);

            return CommandResponse.Success(user.Claims);
        }
    }
}