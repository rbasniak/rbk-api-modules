using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace rbkApiModules.Identity.Core;

public class SwitchTenant : IEndpoint   
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/switch-tenant", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization()
        .WithName("Switch Tenant")
        .WithTags("Authentication");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public string Tenant { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly ITenantsService _tenantsService;
        private readonly RbkAuthenticationOptions _authOptions;

        public Validator(IAuthService usersService, ITenantsService tenantsService, ILocalizationService localization, RbkAuthenticationOptions authOptions)
        {
            _authOptions = authOptions;
            _usersService = usersService;
            _tenantsService = tenantsService;

            RuleFor(x => x.Tenant)
                .NotEmpty()
                .MustAsync(ExistInDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound))
                .MustAsync(BePartOfDomain)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UnauthorizedAccess));
        }

        public async Task<bool> ExistInDatabase(Request request, string destinationDomain, CancellationToken cancellationToken)
        {
            return await _tenantsService.FindAsync(destinationDomain, cancellationToken) != null; 
        }

        public async Task<bool> BePartOfDomain(Request request, string destinationDomain, CancellationToken cancellationToken)
        {
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication ||
                _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.Custom;

            var userExists = await _usersService.FindUserAsync(request.Identity.Username, destinationDomain, cancellationToken) != null;

            return userExists || userWillBeAutomaticallyCreated;
        }
    }

    public class Handler(IJwtFactory _jwtFactory, IAuthService _usersService, IEnumerable<ICustomClaimHandler> _claimHandlers, 
        IOptions<JwtIssuerOptions> jwtOptions, IAutomaticUserCreator _automaticUserCreator, ILogger<Handler> _logger) : ICommandHandler<Request>
    {
        private readonly JwtIssuerOptions _jwtOptions = jwtOptions.Value;

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var user = await _usersService.FindUserAsync(request.Identity.Username, request.Tenant, cancellationToken);

            if (user == null)
            {
                await _automaticUserCreator.CreateIfAllowedAsync(request.Identity.Username, request.Tenant, cancellationToken);
            }

            var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

            await _usersService.UpdateRefreshTokenAsync(request.Identity.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellationToken);

            user = await _usersService.GetUserWithDependenciesAsync(request.Identity.Username, request.Tenant, cancellationToken);

            _logger.LogInformation($"Switching domain for user {user.Username}");

            var extraClaims = new Dictionary<string, string[]>
            {
                { JwtClaimIdentifiers.AuthenticationMode, new[] { user.AuthenticationMode.ToString() } }
            };

            foreach (var claimHandler in _claimHandlers)
            {
                foreach (var claim in await claimHandler.GetClaims(user.TenantId, user.Username))
                {
                    extraClaims.Add(claim.Type, new[] { claim.Value });
                }
            }

            var jwt = await TokenGenerator.GenerateAsync(_jwtFactory, user, extraClaims, cancellationToken);

            await _usersService.RefreshLastLogin(user.Username, user.TenantId, cancellationToken);

            return CommandResponse.Success(jwt);
        }
    }
}
