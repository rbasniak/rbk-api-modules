using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class SwitchDomain
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string DestinationDomain { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly ITenantsService _tenantsService;

        public Validator(IAuthService usersService, ITenantsService tenantsService, ILocalizationService localization)
        {
            _usersService = usersService;
            _tenantsService = tenantsService;

            RuleFor(a => a.DestinationDomain)
                .IsRequired(localization)
                .MustAsync(ExistInDatabase)
                    .WithErrorCode(ValidationErrorCodes.NOT_FOUND)
                        .WithMessage(localization.GetValue("Tenant not found"))
                .MustAsync(BePartOfDomain)
                    .WithMessage(localization.GetValue("Access denied"))
                .WithName(localization.GetValue("Destination domain"));
        }

        public async Task<bool> ExistInDatabase(Request request, string destinationDomain, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(destinationDomain, cancellation) != null;
        }

        public async Task<bool> BePartOfDomain(Request request, string destinationDomain, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(request.Identity.Username, destinationDomain, cancellation);
            return user != null;
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
            var user = await _usersService.FindUserAsync(request.Identity.Username, request.DestinationDomain, cancellation);

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
