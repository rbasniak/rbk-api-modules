using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class ConfirmUserEmail
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
        public string ActivationCode { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.ActivationCode)
                .IsRequired(localization);

            RuleFor(x => x.Tenant)
                .IsRequired(localization);

            RuleFor(a => a.Email)
                .IsRequired(localization)
                .MustAsync(BeValidPair).WithMessage(localization.GetValue("Código de ativação inválido"));
        }

        public async Task<bool> BeValidPair(Command command, string email, CancellationToken cancelation)
        {
            var user = await _usersService.FindUserAsync(email, command.Tenant, cancelation);

            if (user == null) return false;

            return user.ActivationCode == command.ActivationCode;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(request.Email, request.Tenant, cancellation);

            await _usersService.ConfirmUserAsync(request.Email, request.Tenant, cancellation);

            _mailingService.SendConfirmationSuccessMail(user.DisplayName, user.Email);

            return CommandResponse.Success();
        }
    }
}