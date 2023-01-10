using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class RenewAccessToken
{
    public class Command : IRequest<CommandResponse>
    {
        public string RefreshToken { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(a => a.RefreshToken)
                .IsRequired(localization)
                .MustAsync(RefreshTokenExistOnDatabase).WithMessage("Refresh token does not exist anymore")
                .MustAsync(TokenMustBeWithinValidity).WithMessage("Refresh token expired")
                .WithName(localization.GetValue("RefreshToken"));
        }

        public async Task<bool> TokenMustBeWithinValidity(Command comman, string refreshToken, CancellationToken cancelation)
        {
            return await _usersService.IsRefreshTokenValidAsync(refreshToken);
        }

        public async Task<bool> RefreshTokenExistOnDatabase(Command command, string refreshToken, CancellationToken cancelation)
        {
            return await _usersService.RefreshTokenExistsOnDatabaseAsync(refreshToken);
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
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

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            var user = await _usersService.GetUserFromRefreshtokenAsync(request.RefreshToken);

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
