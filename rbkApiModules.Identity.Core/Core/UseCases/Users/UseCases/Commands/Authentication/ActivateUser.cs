using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Core;

public class ActivateUser : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/users/activate", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorization(AuthenticationClaims.MANAGE_USERS)
        .WithName("Activate User")
        .WithTags("Authentication");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public string Username { get; set; } = string.Empty;    
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.Username)
                .NotEmpty()
                .MustAsync(UserExistsOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotFound));
        }

        private async Task<bool> UserExistsOnDatabase(Request request, string username, CancellationToken cancellationToken)
        {
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellationToken);

            return user != null;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            await _usersService.ActivateUserAsync(request.Identity.Tenant, request.Username, cancellationToken);

            var user = await _usersService.GetUserWithDependenciesAsync(request.Username, request.Identity.Tenant, cancellationToken);

            return CommandResponse.Success(UserDetails.FromModel(user));
        }
    }
}
