using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Core;

public class RenameRole : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/authorization/roles", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization()
        .WithName("Rename Role")
        .WithTags("Roles");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
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
                .NotEmpty()
                .MustAsync(NameBeUnique).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.NameAlreadyUsed))
                .MustAsync(NotBeTheDefaultUserRole).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.CannotRenameDefaultUserRole));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        }

        private async Task<bool> NotBeTheDefaultUserRole(Request request, string name, CancellationToken cancellationToken)
        {
            if (!_authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.Credentials ||
                !_authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode != LoginMode.Custom)
            {
                return true;
            }

            var role = await _rolesService.FindAsync(request.Id, cancellationToken);
            
            return role.Name.ToLower() != _authOptions._defaultRoleName.ToLower();
        }

        private async Task<bool> NameBeUnique(Request request, string name, CancellationToken cancellationToken)
        {
            var existingRoles = await _rolesService.FindByNameAsync(name, cancellationToken);

            if (existingRoles.Count() == 0) return true;

            var usedRoles = existingRoles.Where(x => x.TenantId == request.Identity.Tenant || (x.HasNoTenant && request.Identity.HasNoTenant)).ToList();

            return usedRoles.Count == 0 || usedRoles.First().Id == request.Id;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IRolesService _rolesService;

        public Handler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            await _rolesService.RenameAsync(request.Id, request.Name, cancellationToken);

            var role = await _rolesService.GetDetailsAsync(request.Id, cancellationToken);

            return CommandResponse.Success(RoleDetails.FromModel(role));
        }
    }
}