using FluentValidation;
using MediatR;
using System.Diagnostics;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class RenameRole
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IRolesService _rolesService;
        private readonly RbkAuthenticationOptions _authOptions;

        public Validator(IRolesService rolesService, ITenantsService tenantsService, ILocalizationService localization, RbkAuthenticationOptions authOptions)
        {
            _authOptions = authOptions;
            _rolesService = rolesService;

            RuleFor(x => x.Id)
                .RoleExistOnDatabaseForTheCurrentTenant(rolesService, localization);

            RuleFor(x => x.Name)
                .IsRequired(localization)
                .MustAsync(NameBeUnique).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.NameAlreadyUsed))
                .MustAsync(NotBeTheDefaultUserRole).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.CannotRenameDefaultUserRole))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Name));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        }

        private async Task<bool> NotBeTheDefaultUserRole(Request request, string name, CancellationToken cancellation)
        {
            if (!_authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode != LoginMode.WindowsAuthentication) return true;

            var role = await _rolesService.FindAsync(request.Id);
            
            return role.Name.ToLower() != _authOptions._defaultRoleName.ToLower();
        }

        private async Task<bool> NameBeUnique(Request request, string name, CancellationToken cancellation)
        {
            var existingRoles = await _rolesService.FindByNameAsync(name, cancellation);

            if (existingRoles.Count() == 0) return true;

            var usedRoles = existingRoles.Where(x => x.TenantId == request.Identity.Tenant || (x.HasNoTenant && request.Identity.HasNoTenant)).ToList();

            Debug.Assert(usedRoles.Count() <= 1);

            return usedRoles.Count == 0 || usedRoles.First().Id == request.Id;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IRolesService _rolesService;

        public Handler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _rolesService.RenameAsync(request.Id, request.Name, cancellation);

            var role = await _rolesService.GetDetailsAsync(request.Id, cancellation);

            return CommandResponse.Success(role);
        }
    }
}