﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class ReplaceUserRoles
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
        public Guid[] RoleIds { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;
        private readonly IRolesService _rolesService;

        public Validator(IAuthService authService, IRolesService rolesService, ILocalizationService localization)
        {
            _authService = authService;
            _rolesService = rolesService;

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserExistInDatabaseUnderTheSameTenant)
                .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotFound))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User))
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleIds)
                        .NotNull()
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleListMustNotBeEmpty))
                        .DependentRules(() =>
                        {
                            RuleForEach(x => x.RoleIds)
                               .MustAsync(RoleExistOnDatabase)
                               .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound))
                               .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Role));
                        });
                });
        }

        private async Task<bool> RoleExistOnDatabase(Request request, Guid roleId, CancellationToken cancellation)
        {
            var role = await _rolesService.FindAsync(roleId, cancellation);

            return role != null;
        }

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Request request, string username, CancellationToken cancellation)
        {
            return await _authService.FindUserAsync(username, request.Identity.Tenant, cancellation) != null;
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
            var user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellation);

            return CommandResponse.Success(user);
        }
    }
}