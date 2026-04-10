using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Core;

public class ReplaceUserRoles : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authorization/users/set-roles", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.MANAGE_USER_ROLES)
        .WithName("Replace User Roles")
        .WithTags("Authorization");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public string Username { get; set; } = string.Empty;
        public Guid[] RoleIds { get; set; } = [];
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;
        private readonly IRolesService _rolesService;

        public Validator(IAuthService authService, IRolesService rolesService, ILocalizationService localization)
        {
            _authService = authService;
            _rolesService = rolesService;

            RuleFor(x => x.Username)
                .NotEmpty()
                .MustAsync(UserExistInDatabaseUnderTheSameTenant)
                .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotFound))
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleIds)
                        .NotNull()
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleListMustNotBeEmpty))
                        .DependentRules(() =>
                        {
                            RuleForEach(x => x.RoleIds)
                               .MustAsync(RoleExistOnDatabase)
                               .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound));
                        });
                });
        }

        private async Task<bool> RoleExistOnDatabase(Request request, Guid roleId, CancellationToken cancellationToken)
        {
            var role = await _rolesService.FindAsync(roleId, cancellationToken);

            return role != null;
        }

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Request request, string username, CancellationToken cancellationToken)
        {
            return await _authService.FindUserAsync(username, request.Identity.Tenant, cancellationToken) != null;
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
            var user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellationToken);

            return CommandResponse.Success(UserDetails.FromModel(user));
        }
    }
}