using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;
public class RequestPasswordReset
{
    public class Request : IRequest<CommandResponse>
    {
        private string _tenant;

        public string Tenant
        {
            get => _tenant;
            set
            {
                _tenant = value?.ToUpper();
            }
        }
        public string Email { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Tenant)
                .IsRequired(localization)
                .WithName(localization.GetValue(AuthenticationMessages.Fields.Tenant))
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .IsRequired(localization)
                        .MustBeEmail(localization)
                        .MustAsync(EmailBeRegistered).WithMessage(localization.GetValue(AuthenticationMessages.Validations.EmailNotFound))
                        .MustAsync(EmailBeConfirmed).WithMessage(localization.GetValue(AuthenticationMessages.Validations.EmailNotYetConfirmed))
                        .WithName(localization.GetValue(AuthenticationMessages.Fields.Email));
                });

        }

        private async Task<bool> EmailBeRegistered(Request request, string email, CancellationToken cancelation = default)
        {
            return await _authService.IsUserRegisteredAsync(email, request.Tenant, cancelation);
        }

        private async Task<bool> EmailBeConfirmed(Request request, string email, CancellationToken cancellation)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, request.Tenant, cancellation);

            return isConfirmed;
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
            await _authService.RequestPasswordResetAsync(request.Email, request.Tenant, cancellation);

            var user = await _authService.FindUserAsync(request.Email, request.Tenant, cancellation);

            _mailingService.SendPasswordResetMail(user.DisplayName, user.Email, user.PasswordRedefineCode.Hash);

            return CommandResponse.Success();
        }
    }
}