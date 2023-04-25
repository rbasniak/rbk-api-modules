using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;

namespace rbkApiModules.Identity.Core;

public class RenewAccessToken
{
    public class Request : IRequest<CommandResponse>
    {
        public string RefreshToken { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.RefreshToken)
                .IsRequired(localization)
                .MustAsync(RefreshTokenExistOnDatabase).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RefreshTokenNotFound))
                .MustAsync(TokenMustBeWithinValidity).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RefreshTokenExpired))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.RefreshToken));
        }

        public async Task<bool> TokenMustBeWithinValidity(Request comman, string refreshToken, CancellationToken cancelation)
        {
            Log.Logger.Information("Validation: Verifying refresh token validity");

            var isRefreshTokenValid = await _usersService.IsRefreshTokenValidAsync(refreshToken, cancelation);

            if (isRefreshTokenValid)
            {
                Log.Logger.Information("Refresh token is valid");
            }
            else
            {
                Log.Logger.Information("Refresh token is not valid");
            }

            return isRefreshTokenValid;
        }

        public async Task<bool> RefreshTokenExistOnDatabase(Request request, string refreshToken, CancellationToken cancelation)
        {
            Log.Logger.Information("Validation: Checking if refresh token exists on database");

            var tokenExists = await _usersService.RefreshTokenExistsOnDatabaseAsync(refreshToken, cancelation);

            if (tokenExists)
            {
                Log.Logger.Information("Refresh token was found");
            }
            else
            {
                Log.Logger.Information("Refresh token was not found");
            }

            return tokenExists;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IJwtFactory _jwtFactory;
        private readonly IAuthService _usersService;
        private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;

        public Handler(IJwtFactory jwtFactory, IAuthService usersService, IEnumerable<ICustomClaimHandler> claimHandlers)
        {
            _jwtFactory = jwtFactory;
            _usersService = usersService;
            _claimHandlers = claimHandlers;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _usersService.GetUserFromRefreshtokenAsync(request.RefreshToken, cancellation);
            
            Log.Information($"Renewing access token for user {user.Username}");

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

            Log.Information($"New token is {jwt.AccessToken}");

            return CommandResponse.Success(jwt);
        }
    }
}
