using Microsoft.Extensions.Logging;
using rbkApiModules.Identity.Core.DataTransfer.Users;

namespace rbkApiModules.Identity.Core;

public class CreateUser : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/users/create", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .RequireAuthorizationClaim(AuthenticationClaims.MANAGE_USERS)
        .WithName("Create User")
        .WithTags("Authentication");
    }

    public class Request : AuthenticatedRequest, ICommand, IUserMetadata
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string PasswordConfirmation { get; set; } = string.Empty;
        public Guid[] RoleIds { get; set; } = [];
        public Dictionary<string, string> Metadata { get; set; } = new();
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly IRolesService _rolesService;
        private readonly AuthenticationMode _authenticationMode;
        private readonly ILogger<Validator> _logger;
        public Validator(IAuthService usersService, 
            ILocalizationService localization, 
            IEnumerable<ICustomPasswordPolicyValidator> passwordValidators,
            IEnumerable<ICustomUserMetadataValidator> metadataValidators, 
            IRolesService rolesService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<Validator> logger)
        {
            _usersService = usersService;
            _rolesService = rolesService;
            _logger = logger;

            var authenticationModeClaim = httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode);

            _logger.LogInformation("Looking for authentication mode in access token");

            if (authenticationModeClaim == null)
            {
                throw new Exception("Cannot create a new user using a token without authentication mode");
            }

            _authenticationMode = Enum.Parse<AuthenticationMode>(authenticationModeClaim.Value);

            RuleFor(x => x.Username)
                .NotEmpty()
                .MustAsync(UserDoesNotExistOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserAlreadyExists));

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(EmailDoesNotExistOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailAlreadyUsed));

            RuleFor(x => x.DisplayName)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(255);

            RuleFor(x => x)
                        .UserMetadataIsValid(metadataValidators, localization);

            RuleFor(x => x.Password)
                .Must(PasswordsIsRequired)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordIsRequired))
                .Must(PasswordsBeTheSame)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordsMustBeTheSame))
                .PasswordPoliciesAreValid(passwordValidators, localization);

            RuleFor(x => x.RoleIds)
                .NotNull()
                .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleListMustNotBeEmpty))
                .DependentRules(() =>
                {
                    RuleFor(x => x.RoleIds.Length)
                        .GreaterThan(0)
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleListMustNotBeEmpty));

                    RuleForEach(x => x.RoleIds)
                        .MustAsync(RoleExistOnDatabase)
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound));
                });
        }

        private async Task<bool> EmailDoesNotExistOnDatabase(Request request, string email, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validation: Checking if e-mail is not already used: {email}", email);

            var isRegistered = await _usersService.IsUserRegisteredAsync(email, request.Identity.Tenant, cancellationToken);

            if (isRegistered)
            {
                _logger.LogInformation("E-mail is being used");
            }
            else
            {
                _logger.LogInformation("E-mail is not being used");
            }

            return !isRegistered;
        }

        private async Task<bool> RoleExistOnDatabase(Request request, Guid roleId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validation: Checking is role exists: {Role}", roleId);

            var role = await _rolesService.FindAsync(roleId, cancellationToken);

            var result = role != null;

            if (result)
            {
                _logger.LogInformation("Role found: {Role}", role!.Name);
            }
            else
            {
                _logger.LogInformation("Role not found");
            }

            return result;
        }

        private bool PasswordsBeTheSame(Request request, string _)
        {
            _logger.LogInformation("Validation: Checking is passwords match");

            if (_authenticationMode == AuthenticationMode.Windows)
            {
                _logger.LogInformation("Using Windows Authentication, password is not used in this scenario");

                return true;
            }

            var result = request.PasswordConfirmation == request.Password;

            if (result)
            {
                _logger.LogInformation("Passwords match");
            }
            else
            {
                _logger.LogInformation("Passwords do not match");
            }

            return result;
        }

        private bool PasswordsIsRequired(Request request, string _)
        {
            _logger.LogInformation("Validation: Checking password");

            if (_authenticationMode == AuthenticationMode.Windows)
            {
                _logger.LogInformation("Using Windows Authentication, password is not used in this scenario");

                return true;
            }

            var result = !String.IsNullOrEmpty(request.Password);

            if (result)
            {
                _logger.LogInformation("Password is not empty");
            }
            else
            {
                _logger.LogInformation("Password is empty");
            }

            return result;
        }

        private async Task<bool> UserDoesNotExistOnDatabase(Request request, string username, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Validation: Checking if user exists on database");
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellationToken);
            var result = user == null;

            if (result)
            {
                _logger.LogInformation("User found: {username}", username);
            }
            else
            {
                _logger.LogInformation("User not found: {username}", username);
            }

            return result;
        }
    }

    public class Handler(IAuthService _usersService, IEnumerable<IUserMetadataService> _userMetadataService,
        IAvatarStorage _avatarStorage, IHttpContextAccessor _httpContextAccessor, RbkAuthenticationOptions _options) : ICommandHandler<Request>
    {

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            string avatarUrl = request.Picture;
            string avatarBase64 = string.Empty;

            if (String.IsNullOrEmpty(request.Picture))
            {
                avatarBase64 = AvatarGenerator.GenerateBase64Avatar(request.DisplayName);
            }

            if (!String.IsNullOrEmpty(request.Picture) && !request.Picture.ToLower().StartsWith("http"))
            {
                var filename = await _avatarStorage.SaveAsync(avatarBase64, _options._userAvatarPath, Guid.NewGuid().ToString("N"), cancellationToken);
                avatarUrl = _avatarStorage.GetRelativePath(filename);
            }

            var authenticationModeClaim = _httpContextAccessor.HttpContext!.User.Claims.FirstOrDefault(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode);

            if (authenticationModeClaim == null)
            {
                throw new Exception("Cannot create user using a token without authentication mode");
            }

            var authenticationMode = Enum.Parse<AuthenticationMode>(_httpContextAccessor.HttpContext.User.Claims.First(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode).Value);

            var user = await _usersService.CreateUserAsync(
                tenant: request.Identity.Tenant,
                username: request.Username,
                password: request.Password,
                email: request.Email,
                displayName: request.DisplayName,
                avatar: avatarUrl,
                isConfirmed: true,
                authenticationMode: authenticationMode,
                metadata: request.Metadata,
                cancellationToken: cancellationToken);

            var allMetadata = new Dictionary<string, string>();

            foreach (var metadataService in _userMetadataService)
            {
                var data = await metadataService.GetIdentityInfo(user);

                foreach (var kvp in data)
                {
                    if (allMetadata.ContainsKey(kvp.Key))
                    {
                        allMetadata[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        allMetadata.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            await _usersService.AppendUserMetadata(request.Username, request.Identity.Tenant, allMetadata, cancellationToken);

            user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellationToken);

            return CommandResponse.Success(UserDetails.FromModel(user));
        }
    }
}
