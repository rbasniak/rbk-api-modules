using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class DeleteTenant
{
    public class Command : IRequest<CommandResponse>
    {
        private string _alias;

        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value.ToUpper();
            }
        }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly ITenantsService _tenantsService;

        public Validator(ITenantsService tenantsService, ILocalizationService localization)
        {
            _tenantsService = tenantsService;

            RuleFor(a => a.Alias)
                .IsRequired(localization)
                .MustAsync(ExistInDatabase).WithErrorCode(ValidationErrorCodes.NOT_FOUND).WithMessage(localization.GetValue("Tenant not found"))
                .WithName(localization.GetValue("Alias"));
        }

        private async Task<bool> ExistInDatabase(Command command, string alias, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(alias, cancellation) != null;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly ITenantsService _tenantsService;
        private readonly IRolesService _rolesService;
        private readonly IAuthService _authService;

        public Handler(ITenantsService tenantsService, IAuthService authService, IRolesService rolesService)
        {
            _tenantsService = tenantsService;
            _rolesService = rolesService;
            _authService = authService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _authService.DeleteUsersFromTenant(request.Alias, cancellation);

            await _rolesService.DeleteRolesFromTenant(request.Alias, cancellation);

            await _tenantsService.DeleteAsync(request.Alias, cancellation);

            return CommandResponse.Success();
        }
    }
}