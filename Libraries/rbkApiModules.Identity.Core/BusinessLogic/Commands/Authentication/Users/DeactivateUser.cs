﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class DeativateUser
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserExistsOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotFound))
                .Must(UserCannotDeactivateItself)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserCannotDeactivateItselft))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User));
        }

        private bool UserCannotDeactivateItself(Request request, string username)
        {
            return username.ToLower() != request.Identity.Username.ToLower();
        }

        private async Task<bool> UserExistsOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellation);

            return user != null;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _usersService.DeactivateUserAsync(request.Identity.Tenant, request.Username, cancellation);

            return CommandResponse.Success();
        }
    }
}
