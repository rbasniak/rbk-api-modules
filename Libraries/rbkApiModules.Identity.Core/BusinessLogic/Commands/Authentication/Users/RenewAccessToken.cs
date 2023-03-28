using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

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
            return await _usersService.IsRefreshTokenValidAsync(refreshToken, cancelation);
        }

        public async Task<bool> RefreshTokenExistOnDatabase(Request request, string refreshToken, CancellationToken cancelation)
        {
            return await _usersService.RefreshTokenExistsOnDatabaseAsync(refreshToken, cancelation);
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

            var extraClaims = new Dictionary<string, string[]>();

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
