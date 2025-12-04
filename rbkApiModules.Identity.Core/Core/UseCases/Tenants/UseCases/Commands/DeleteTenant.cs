namespace rbkApiModules.Identity.Core;

public class DeleteTenant : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapDelete("/api/authorization/tenants/{tenantId}", async (string tenantId, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(new Request { Alias = tenantId }, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization(AuthenticationClaims.MANAGE_TENANTS)
        .WithName("Delete Tenant")
        .WithTags("Tenants");
    } 

    public class Request : ICommand
    {
        private string _alias = string.Empty;

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

    public class Validator : AbstractValidator<Request>
    {
        private readonly ITenantsService _tenantsService;

        public Validator(ITenantsService tenantsService, ILocalizationService localization)
        {
            _tenantsService = tenantsService;

            RuleFor(x => x.Alias)
                .NotEmpty()
                .MustAsync(ExistInDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound));
        }

        private async Task<bool> ExistInDatabase(Request request, string alias, CancellationToken cancellationToken)
        {
            return await _tenantsService.FindAsync(alias, cancellationToken) != null;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly ITenantsService _tenantsService;
        private readonly IRolesService _rolesService;
        private readonly IAuthService _authService;
        private readonly ILocalizationService _localization;

        public Handler(ITenantsService tenantsService, IAuthService authService, IRolesService rolesService, ILocalizationService localization)
        {
            _tenantsService = tenantsService;
            _rolesService = rolesService;
            _authService = authService;
            _localization = localization;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            // MIGRATION: create transaction
            try
            {
                await _authService.DeleteUsersFromTenant(request.Alias, cancellationToken);

                await _rolesService.DeleteRolesFromTenant(request.Alias, cancellationToken);

                await _tenantsService.DeleteAsync(request.Alias, cancellationToken);

                return CommandResponse.Success();
            }
            catch 
            {
                throw new ExpectedInternalException(_localization.LocalizeString(AuthenticationMessages.Erros.CannotDeleteUsedTenant));
            }
        } 
    }
}