using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;

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
        private readonly RbkAuthenticationOptions _authOptions;

        public Validator(IAuthService usersService, ITenantsService tenantsService, ILocalizationService localization, RbkAuthenticationOptions authOptions)
        {
            _authOptions = authOptions;
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
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication;

            var userExists = await _usersService.FindUserAsync(request.Identity.Username, destinationDomain, cancellation) != null;

            return userExists || userWillBeAutomaticallyCreated;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IAuthService _usersService;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IAutomaticUserCreator _automaticUserCreator;
        private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;

        public Handler(IJwtFactory jwtFactory, IAuthService usersService, IEnumerable<ICustomClaimHandler> claimHandlers, 
            IOptions<JwtIssuerOptions> jwtOptions, IAutomaticUserCreator automaticUserCreator)
        {
            _jwtFactory = jwtFactory;
            _usersService = usersService;
            _jwtOptions = jwtOptions.Value;
            _claimHandlers = claimHandlers;
            _automaticUserCreator = automaticUserCreator;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(request.Identity.Username, request.Tenant, cancellation);

            if (user == null)
            {
                await _automaticUserCreator.CreateIfAllowedAsync(request.Identity.Username, request.Tenant, cancellation);
            }

            var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

            await _usersService.UpdateRefreshTokenAsync(request.Identity.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellation);

            user = await _usersService.GetUserWithDependenciesAsync(request.Identity.Username, request.Tenant, cancellation);

            Log.Information($"Switching domain for user {user.Username}");

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

            var jwt = await TokenGenerator.GenerateAsync(_jwtFactory, user, extraClaims);

            await _usersService.RefreshLastLogin(user.Username, user.TenantId, cancellation);

            return CommandResponse.Success(jwt);
        }
    }
}
