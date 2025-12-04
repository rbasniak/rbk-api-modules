using rbkApiModules.Identity.Core.DataTransfer;

namespace rbkApiModules.Identity.Core;

public class UpdateRoleClaims : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/roles/update-claims", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization()
        .WithName("Update Role Claims")
        .WithTags("Roles");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public Guid Id { get; set; }
        public Guid[] ClaimsIds { get; set; } = [];
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, IRolesService rolesService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(x => x.Id)
                .RoleExistOnDatabaseForTheCurrentTenant(rolesService, localization)
                .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound));

            RuleFor(x => x.ClaimsIds)
                .NotNull().WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimListMustNotBeEmpty));

            RuleForEach(x => x.ClaimsIds)
                .MustAsync(ClaimExistInDatabase).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UnknownClaimInTheList));

            RuleFor(x => x.Identity)
                .TenantExistOnDatabase(tenantsService, localization)
                .HasCorrectRoleManagementAccessRights(localization);
        }  

        private async Task<bool> ClaimExistInDatabase(Request request, Guid id, CancellationToken cancellationToken)
        {
            return await _claimsService.FindAsync(id, cancellationToken) != null;
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
            await _rolesService.UpdateRoleClaims(request.Id, request.ClaimsIds, cancellationToken);

            var role = await _rolesService.GetDetailsAsync(request.Id, cancellationToken);

            return CommandResponse.Success(RoleDetails.FromModel(role));
        }
    }
}