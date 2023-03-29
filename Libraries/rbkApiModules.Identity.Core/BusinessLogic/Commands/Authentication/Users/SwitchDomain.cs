using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class SwitchTenant
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Tenant { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly ITenantsService _tenantsService;

        public Validator(IAuthService usersService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _usersService = usersService;
            _tenantsService = tenantsService;

            RuleFor(x => x.Tenant)
                .IsRequired(localization)
                .MustAsync(ExistInDatabase)
                    .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound))
                .MustAsync(BePartOfDomain)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UnauthorizedAccess))
                    .WithName(localization.LocalizeString(AuthenticationMessages.Fields.DestinationDomain));
        }

        public async Task<bool> ExistInDatabase(Request request, string destinationDomain, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(destinationDomain, cancellation) != null; 
        }

        public async Task<bool> BePartOfDomain(Request request, string destinationDomain, CancellationToken cancellation)
        {
            return await _usersService.FindUserAsync(request.Identity.Username, destinationDomain, cancellation) != null;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IAuthService _usersService;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;

        public Handler(IJwtFactory jwtFactory, IAuthService usersService, IEnumerable<ICustomClaimHandler> claimHandlers, IOptions<JwtIssuerOptions> jwtOptions, IHttpContextAccessor httpContextAccessor)
        {
            _jwtFactory = jwtFactory;
            _usersService = usersService;
            _claimHandlers = claimHandlers;
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

            await _usersService.UpdateRefreshTokenAsync(request.Identity.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellation);

            var user = await _usersService.GetUserWithDependenciesAsync(request.Identity.Username, request.Tenant, cancellation);

            var extraClaims = new Dictionary<string, string[]>();

            var authenticationMode = _httpContextAccessor.HttpContext.User.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value;

            extraClaims.Add(JwtClaimIdentifiers.AuthenticationMode, new[] { authenticationMode });

            foreach (var claimHandler in _claimHandlers)
            {
                foreach (var claim in await claimHandler.GetClaims(user.TenantId, user.Username))
                {
                    extraClaims.Add(claim.Type, new[] { claim.Value });
                }
            }

            var jwt = TokenGenerator.Generate(_jwtFactory, user, extraClaims);

            await _usersService.RefreshLastLogin(user.Username, user.TenantId, cancellation);

            return CommandResponse.Success(jwt);
        }
    }
}
