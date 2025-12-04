namespace rbkApiModules.Identity.Core;

public class CreateTenant : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/tenants", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization(AuthenticationClaims.MANAGE_TENANTS)
        .WithName("Create Tenant")
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
                _alias = value?.ToUpper();
            }
        }
        public string Name { get; set; } = string.Empty;
        public string Metadata { get; set; } = string.Empty;

        public AdminUser AdminInfo { get; set; } = new();
    }

    public class AdminUser
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly ITenantsService _tenantsService;

        public Validator(ITenantsService tenantsService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> validators)
        {
            _tenantsService = tenantsService;

            RuleFor(x => x.AdminInfo)
                .NotNull()
                .DependentRules(() => 
                {
                    RuleFor(x => x.AdminInfo.Username)
                        .NotEmpty();

                    RuleFor(x => x.AdminInfo.Email)
                        .NotEmpty();

                    RuleFor(x => x.AdminInfo.Password)
                        .PasswordPoliciesAreValid(validators, localization);

                    RuleFor(x => x.AdminInfo.DisplayName)
                        .NotEmpty();

                    RuleFor(x => x.Alias)
                        .NotEmpty()
                        .MustAsync(NameBeUnique)
                            .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantAliasAlreadyUsed))
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Name)
                                .NotEmpty()
                                .MustAsync(NameNotBeingUsed)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.NameAlreadyUsed));
                        });
                });

        }

        private async Task<bool> NameBeUnique(Request request, string alias, CancellationToken cancellationToken)
        {
            return await _tenantsService.FindAsync(alias, cancellationToken) == null;
        }

        private async Task<bool> NameNotBeingUsed(Request request, string name, CancellationToken cancellationToken)
        {
            var existingTenant = await _tenantsService.FindByNameAsync(name, cancellationToken);

            return existingTenant == null || existingTenant.Alias == request.Alias;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly ITenantsService _tenantsService;
        private readonly IClaimsService _claimsService;
        private readonly IAuthService _usersService;

        public Handler(ITenantsService tenantsService, IAuthService usersService, IClaimsService claimsService)
        {
            _tenantsService = tenantsService;
            _claimsService = claimsService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var tenant = new Tenant(request.Alias, request.Name, request.Metadata);

            tenant = await _tenantsService.CreateAsync(tenant, cancellationToken);

            await _usersService.CreateUserAsync(
                tenant: request.Alias,
                username: request.AdminInfo.Username,
                password: request.AdminInfo.Password,
                email: request.AdminInfo.Email,
                displayName: request.AdminInfo.DisplayName,
                avatar: AvatarGenerator.GenerateBase64Avatar(request.AdminInfo.DisplayName),
                isConfirmed: true,
                authenticationMode: AuthenticationMode.Credentials,
                metadata: new Dictionary<string, string>(),
                cancellationToken
            );

            var claims = await _claimsService.GetAllAsync(cancellationToken);

            var desiredClaims = new List<Guid>
            {
                claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USERS).Id,
                claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USER_ROLES).Id,
                claims.First(x => x.Identification == AuthenticationClaims.OVERRIDE_USER_CLAIMS).Id,
                claims.First(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES).Id
            };

            await _claimsService.AddClaimOverridesAsync(desiredClaims.ToArray(), request.AdminInfo.Username, tenant.Alias, ClaimAccessType.Allow, cancellationToken);

            return CommandResponse.Success(TenantDetails.FromModel(tenant));
        }
    }
}