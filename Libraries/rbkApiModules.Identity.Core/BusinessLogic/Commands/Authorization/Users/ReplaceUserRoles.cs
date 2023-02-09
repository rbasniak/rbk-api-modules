using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class ReplaceUserRoles
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
        public Guid[] RoleIds { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _authService;
        private readonly IRolesService _rolesService;

        public Validator(IAuthService authService, IRolesService rolesService, ILocalizationService localization)
        {
            _authService = authService;
            _rolesService = rolesService;

            RuleFor(a => a.Username)
                .IsRequired(localization)
                .MustAsync(UserExistInDatabaseUnderTheSameTenant).WithMessage(localization.GetValue("User not found."))
                .WithName(localization.GetValue("User"))
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleIds)
                        .NotNull().WithMessage("The list of roles must be provided")
                        .DependentRules(() =>
                        {
                            RuleForEach(a => a.RoleIds)
                               .MustAsync(RoleExistOnDatabase).WithMessage("Não foi possível localizar o role no servidor")
                               .WithName("Controle de Acesso");
                        });
                });
        }

        private async Task<bool> RoleExistOnDatabase(Command command, Guid roleId, CancellationToken cancellation)
        {
            var role = await _rolesService.FindAsync(roleId, cancellation);

            return role != null;
        }

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Command command, string username, CancellationToken cancellation)
        {
            return await _authService.FindUserAsync(username, command.Identity.Tenant, cancellation) != null;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            var user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellation);

            return CommandResponse.Success(user);
        }
    }
}