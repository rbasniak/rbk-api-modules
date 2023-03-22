using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class ResendEmailConfirmation
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Email { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Email)
                .IsRequired(localization)
                .MustBeEmail(localization)
                .MustAsync(EmailBeRegistered)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.EmailNotFound))
                .MustAsync(EmailBeUnconfirmed)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.EmailAlreadyConfirmed));
        }

        private async Task<bool> EmailBeRegistered(Request request, string email, CancellationToken cancellation)
        {
            return await _authService.IsUserRegisteredAsync(email, request.Identity.Tenant, cancellation);
        }

        private async Task<bool> EmailBeUnconfirmed(Request request, string email, CancellationToken cancellation)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, request.Identity.Tenant, cancellation);

            return !isConfirmed;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _authService;

        public Handler(IAuthService authService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _authService = authService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _authService.FindUserAsync(request.Email, request.Identity.Tenant);

            _mailingService.SendConfirmationMail(user.DisplayName, user.Email, user.ActivationCode);

            return CommandResponse.Success();
        }
    }
}
