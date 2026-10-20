using System.Text.Json.Serialization;

namespace rbkApiModules.Identity.Core;

public class UserLogin
{
    public static void MapCredentialsLoginEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/login", async (Request request, IDispatcher dispatcher, 
            HttpContext httpContext, RbkDefaultAdminOptions adminOptions, CancellationToken cancellationToken) =>
        {
            request.AuthenticationMode = AuthenticationMode.Credentials;
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .AllowAnonymous()
        .WithName("Login with Credentials")
        .WithTags("Authentication");
    }

    public class Request : ICommand, ILoginData
    {
        private string _tenant = null;

        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Tenant
        {
            get => _tenant;
            set
            {
                _tenant = value?.ToUpper();
            }
        }

        [JsonIgnore]
        public AuthenticationMode AuthenticationMode { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _userService;
        private readonly RbkAuthenticationOptions _authOptions;

        public Validator(IAuthService userService, ILocalizationService localization, IEnumerable<ICustomLoginPolicyValidator> loginPolicies, RbkAuthenticationOptions authOptions)
        {
            _userService = userService;
            _authOptions = authOptions;

            RuleFor(x => x)
                .LoginPoliciesAreValid(loginPolicies, localization).DependentRules(() => 
                {
                    RuleFor(x => x.Username)
                        .NotEmpty()
                        .MustAsync(ExistOnDatabase)
                            .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidCredentials))
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Username)
                                .MustAsync(BeActive)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.AccountDeactivated))
                                .MustAsync(UseCorrectAuthenticationMode)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidLoginMode))
                                .MustAsync(BeConfirmed)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotYetConfirmed));

                            RuleFor(x => x.Password)
                                .NotEmpty()
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordIsRequired))
                                .MustAsync(MatchPassword)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidCredentials));
                        });
                });
        }

        private static bool UserWillBeAutomaticallyCreated(RbkAuthenticationOptions authOptions) =>
            authOptions._allowUserCreationOnFirstAccess && authOptions._loginMode == LoginMode.Custom;

        public async Task<bool> ExistOnDatabase(Request request, string username, CancellationToken cancellationToken)
        {
            var userWillBeAutomaticallyCreated = UserWillBeAutomaticallyCreated(_authOptions);

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellationToken);

            return user != null || userWillBeAutomaticallyCreated;
        }

        public async Task<bool> BeConfirmed(Request request, string username, CancellationToken cancellationToken)
        {
            var userWillBeAutomaticallyCreated = UserWillBeAutomaticallyCreated(_authOptions);

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellationToken);

            return user != null && user.IsConfirmed || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> BeActive(Request request, string username, CancellationToken cancellationToken)
        {
            var userWillBeAutomaticallyCreated = UserWillBeAutomaticallyCreated(_authOptions);

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellationToken);

            return user != null && user.IsActive || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> UseCorrectAuthenticationMode(Request request, string username, CancellationToken cancellationToken)
        {
            var userWillBeAutomaticallyCreated = UserWillBeAutomaticallyCreated(_authOptions);

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellationToken);

            return user != null && user.AuthenticationMode == request.AuthenticationMode || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> MatchPassword(Request request, string password, CancellationToken cancellationToken)
        {
            var user = await _userService.FindUserAsync(request.Username, request.Tenant, cancellationToken);

            return PasswordHasher.VerifyPassword(password, user.Password);
        }
    }

    public class Handler(IUserAuthenticator _tokenCreator) : ICommandHandler<Request>
    {
        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var jwt = await _tokenCreator.Authenticate(request.Username, request.Tenant, jwtOptionsOverride: null, cancellationToken);

            return CommandResponse.Success(jwt);
        }
    }
}

public enum AuthenticationMode
{
    Credentials = 0,
    [Obsolete("Windows authentication is no longer supported. Existing database values are retained for compatibility.")]
    Windows = 1,
    Custom = 2,
    ApiKey = 3
}
