using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;
public class RequestPasswordReset
{
    public class Command : IRequest<CommandResponse>
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

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Tenant)
                .IsRequired(localization)
                .WithName(localization.GetValue("Tenant"))
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .IsRequired(localization)
                        .MustBeEmail(localization)
                        .MustAsync(EmailBeRegistered).WithMessage(localization.GetValue("E-mail not found"))
                        .MustAsync(EmailBeConfirmed).WithMessage(localization.GetValue("E-mail não confirmado"))
                        .WithName(localization.GetValue("Email"));
                });

        }

        private async Task<bool> EmailBeRegistered(Command command, string email, CancellationToken cancelation = default)
        {
            return await _authService.IsUserRegisteredAsync(email, command.Tenant, cancelation);
        }

        private async Task<bool> EmailBeConfirmed(Command command, string email, CancellationToken cancellation)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, command.Tenant, cancellation);

            return isConfirmed;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _authService;

        public Handler(IAuthService authService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _authService = authService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _authService.RequestPasswordResetAsync(request.Email, request.Tenant, cancellation);

            var user = await _authService.FindUserAsync(request.Email, request.Tenant, cancellation);

            _mailingService.SendPasswordResetMail(user.DisplayName, user.Email, user.PasswordRedefineCode.Hash);

            return CommandResponse.Success();
        }
    }
}