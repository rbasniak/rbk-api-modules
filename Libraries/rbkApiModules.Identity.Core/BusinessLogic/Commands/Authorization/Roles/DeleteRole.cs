using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class DeleteRole
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IRolesService _rolesService;

        public Validator(IRolesService rolesService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _rolesService = rolesService;

            RuleFor(x => x.Id)
                .RoleExistOnDatabaseForTheCurrentTenant(rolesService, localization)
                .MustAsync(NotBeUsedInAnyUserUnlessThereIsAnAlternateRole)
                .WithMessage(localization.GetValue(AuthenticationMessages.Validations.RoleIsBeingUsed))
            .WithName(localization.GetValue(AuthenticationMessages.Fields.Role));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        } 

        private async Task<bool> NotBeUsedInAnyUserUnlessThereIsAnAlternateRole(Request request, Guid id, CancellationToken cancellation)
        {
            var isUsed = await _rolesService.IsUsedByAnyUsersAsync(id, cancellation);

            // If it is used and is a tenant role, look for an application role to replace it
            if (isUsed && request.Identity.HasTenant)
            {
                var role = await _rolesService.FindAsync(id, cancellation);
                var roles = await _rolesService.FindByNameAsync(role.Name, cancellation);

                var applicationRole = roles.FirstOrDefault(x => x.HasNoTenant);

                return applicationRole != null;
            }
            else
            {
                return !isUsed;
            }
        } 
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IRolesService _rolesService;
        private readonly IAuthService _usersService;

        public Handler(IRolesService rolesService, IAuthService usersService)
        {
            _rolesService = rolesService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var tenantRole = await _rolesService.FindAsync(request.Id, cancellation);

            if (request.Identity.HasTenant)
            {
                var existingRoles = await _rolesService.FindByNameAsync(tenantRole.Name, cancellation);
                var applicationRole = existingRoles.FirstOrDefault(x => x.TenantId == null);

                if (applicationRole != null)
                {
                    var users = await _usersService.GetAllWithRoleAsync(
                        userTenant: request.Identity.Tenant,
                        roleTenant: request.Identity.Tenant, 
                        roleName: applicationRole.Name, 
                        cancellation
                    );

                    foreach (var user in users)
                    {
                        await _usersService.RemoveRole(user.Username, user.TenantId, tenantRole.Id, cancellation);
                        await _usersService.AddRole(user.Username, user.TenantId, applicationRole.Id, cancellation);
                    }
                }
            }

            await _rolesService.DeleteAsync(request.Id, cancellation);

            return CommandResponse.Success();
        }
    }
}