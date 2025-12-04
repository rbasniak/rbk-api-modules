namespace rbkApiModules.Identity.Core;

public class UpdateTenant : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPut("/api/authorization/tenants", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization(AuthenticationClaims.MANAGE_TENANTS)
        .WithName("Update Tenant")
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
                _alias = value?.ToUpper() ?? string.Empty;
            }
        }
        public string Name { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;
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
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound))
                .DependentRules(() => 
                {
                    RuleFor(x => x.Name)
                       .NotEmpty()
                       .MustAsync(NameNotBeingUsed).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.NameAlreadyUsed));
                });
        }

        private async Task<bool> ExistInDatabase(Request request, string alias, CancellationToken cancellationToken)
        {
            return await _tenantsService.FindAsync(alias, cancellationToken) != null;
        }

        private async Task<bool> NameNotBeingUsed(Request request, string name, CancellationToken cancellationToken)
        {
            var existingTenant = await _tenantsService.FindByNameAsync(name, cancellationToken);

            return  existingTenant == null || existingTenant.Alias == request.Alias;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly ITenantsService _tenantsService;

        public Handler(ITenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantsService.UpdateAsync(request.Alias, request.Name, request.Metadata, cancellationToken);

            return CommandResponse.Success(TenantDetails.FromModel(tenant));
        }
    }
}