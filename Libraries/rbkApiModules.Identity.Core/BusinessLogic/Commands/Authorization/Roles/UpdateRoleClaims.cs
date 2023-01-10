using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class UpdateRoleClaims
{
    public class Command : AuthenticatedCommand, IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public Guid[] ClaimsIds { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IRolesService _rolesService;
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, IRolesService rolesService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _rolesService = rolesService;
            _claimsService = claimsService;

            RuleFor(a => a.Id)
                .RoleExistOnDatabaseForTheCurrentTenant(rolesService, localization).WithMessage(localization.GetValue("Role not found"));

            RuleFor(x => x.ClaimsIds)
                .NotNull().WithMessage(localization.GetValue("The list of claims must be provided"));

            RuleForEach(a => a.ClaimsIds)
                .MustAsync(ClaimExistInDatabase).WithMessage(localization.GetValue("Unknown claim in the list"));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        }  

        private async Task<bool> ClaimExistInDatabase(Command command, Guid id, CancellationToken cancelation)
        {
            return await _claimsService.FindAsync(id) != null;
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IRolesService _rolesService;

        public Handler(IRolesService rolesService)
        {
            _rolesService = rolesService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _rolesService.UpdateRoleClaims(request.Id, request.ClaimsIds, cancellation);

            var role = await _rolesService.GetDetailsAsync(request.Id, cancellation);

            return CommandResponse.Success(role);
        }
    }
}