﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class ResendEmailConfirmation
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        private string _tenant;

        public string Email { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Email)
                .IsRequired(localization)
                .MustBeEmail(localization)
                .MustAsync(EmailBeRegistered).WithMessage(localization.GetValue("E-mail não cadastrado"))
                .MustAsync(EmailBeUnconfirmed).WithMessage(localization.GetValue("E-mail já confirmado"));
        }

        private async Task<bool> EmailBeRegistered(Command command, string email, CancellationToken cancellation)
        {
            return await _authService.IsUserRegisteredAsync(email, command.Identity.Tenant, cancellation);
        }

        private async Task<bool> EmailBeUnconfirmed(Command command, string email, CancellationToken cancellation)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, command.Identity.Tenant, cancellation);

            return !isConfirmed;
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
            var user = await _authService.FindUserAsync(request.Email, request.Identity.Tenant);

            _mailingService.SendConfirmationMail(user.DisplayName, user.Email, user.ActivationCode);

            return CommandResponse.Success();
        }
    }
}