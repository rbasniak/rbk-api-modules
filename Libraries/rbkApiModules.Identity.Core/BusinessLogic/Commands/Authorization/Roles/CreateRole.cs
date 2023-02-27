using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class CreateRole
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Name { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IRolesService _rolesService;

        public Validator(IRolesService rolesService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _rolesService = rolesService;

            RuleFor(x => x.Name)
                .IsRequired(localization)
                .MustAsync(NameBeUnique).WithMessage(localization.GetValue("Name already used"))
                .WithName(localization.GetValue("Role"));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        }  

        private async Task<bool> NameBeUnique(Request request, string name, CancellationToken cancellation)
        {
            var roles = await _rolesService.FindByNameAsync(name, cancellation);

            if (roles.Count() == 0) return true;

            var usedRoles = roles.Where(x => x.TenantId == request.Identity.Tenant || (x.HasNoTenant && request.Identity.HasNoTenant)).ToList();

            return usedRoles.Count == 0;
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
            var tenantRole = new Role(request.Identity.Tenant, request.Name);

            tenantRole = await _rolesService.CreateAsync(tenantRole, cancellation);

            if (request.Identity.HasTenant)
            {
                var existingRoles = await _rolesService.FindByNameAsync(tenantRole.Name, cancellation);
                var applicationRole = existingRoles.FirstOrDefault(x => x.TenantId == null);

                if (applicationRole != null)
                {
                    var users = await _usersService.GetAllWithRoleAsync(
                        userTenant: request.Identity.Tenant, 
                        roleTenant: null, 
                        roleName: applicationRole.Name, 
                        cancellation);

                    foreach (var user in users)
                    {
                        await _usersService.AddRole(user.Username, user.TenantId, tenantRole.Id, cancellation);
                        await _usersService.RemoveRole(user.Username, user.TenantId, applicationRole.Id, cancellation);
                    }
                }
            }

            return CommandResponse.Success(tenantRole);
        }
    } 
}