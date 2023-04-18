using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;

namespace rbkApiModules.Identity.Core;

public class CreateUser
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, IUserMetadata
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Picture { get; set; }
        public string PasswordConfirmation { get; set; }
        public Guid[] RoleIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly IRolesService _rolesService;
        private readonly AuthenticationMode _authenticationMode;

        public Validator(IAuthService usersService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> passwordValidators,
            IEnumerable<ICustomUserMetadataValidator> metadataValidators, IRolesService rolesService, IHttpContextAccessor httpContextAccessor)
        {
            _usersService = usersService;
            _rolesService = rolesService;

            var authenticationModeClaim = httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == JwtClaimIdentifiers.AuthenticationMode);

            Log.Information("Looking for authentication mode in access token");

            _authenticationMode = AuthenticationMode.Credentials;

            if (authenticationModeClaim != null)
            {
                Log.Information("Claim found: {value}", authenticationModeClaim.Value);

                if (authenticationModeClaim.Value == "windows")
                {
                    Log.Information("Setting up creation for Windows Authentication scenario");

                    _authenticationMode = AuthenticationMode.Windows;
                }    
                else
                {
                    Log.Information("Setting up creation for Credentials scenario");
                }
            }
            else
            {
                Log.Information("Claim not found. Setting up creation for Credentials scenario");
            }

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserDoesNotExistOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserAlreadyExists))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User));

            RuleFor(x => x.Email)
                .IsRequired(localization)
                .MustBeEmail(localization)
                .MustAsync(EmailDoesNotExistOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailAlreadyUsed))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Email));

            RuleFor(x => x.DisplayName)
                .IsRequired(localization)
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.DisplayName));

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
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.RoleNotFound))
                        .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Role));
                });
        }

        private async Task<bool> EmailDoesNotExistOnDatabase(Request request, string email, CancellationToken cancellation)
        {
            Log.Information("Validation: Checking if e-mail is not already used: {email}", email);

            var isRegistered = await _usersService.IsUserRegisteredAsync(email, request.Identity.Tenant, cancellation);

            if (isRegistered) 
            {
                Log.Information("E-mail is being used");
            }
            else
            {
                Log.Information("E-mail is not being used");
            }

            return !isRegistered;
        } 

        private async Task<bool> RoleExistOnDatabase(Request request, Guid roleId, CancellationToken cancellation)
        {
            Log.Information("Validation: Checking is role exists: {role}", roleId);

            var role = await _rolesService.FindAsync(roleId, cancellation);

            var result = role != null;

            if (result)
            {
                Log.Information("Role found: {role}", role.Name);
            }
            else
            {
                Log.Information("Role not found");
            }

            return result;
        }

        private bool PasswordsBeTheSame(Request request, string _)
        {
            Log.Information("Validation: Checking is passwords match");

            if (_authenticationMode == AuthenticationMode.Windows)
            {
                Log.Information("Using Windows Authentication, password is not used in this scenario");

                return true;
            }

            var result = request.PasswordConfirmation == request.Password;

            if (result)
            {
                Log.Information("Passwords match");
            }
            else
            {
                Log.Information("Passwords do not match");
            }

            return result;
        }

        private bool PasswordsIsRequired(Request request, string _)
        {
            Log.Information("Validation: Checking password");

            if (_authenticationMode == AuthenticationMode.Windows)
            {
                Log.Information("Using Windows Authentication, password is not used in this scenario");

                return true;
            }

            var result = !String.IsNullOrEmpty(request.Password);

            if (result)
            {
                Log.Information("Password is not empty");
            }
            else
            {
                Log.Information("Password is empty");
            }

            return result;
        } 

        private async Task<bool> UserDoesNotExistOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            Log.Information("Validation: Checking if user exists on database");
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellation);
            var result = user == null;

            if (result)
            {
                Log.Information("User found: {username}", username);
            }
            else
            {
                Log.Information("User not found: {username}", username);
            }

            return result;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _usersService;
        private readonly IAvatarStorage _avatarStorage;
        private readonly RbkAuthenticationOptions _options;
        private readonly IEnumerable<IUserMetadataService> _userMetadataService;

        public Handler(IAuthService usersService, IEnumerable<IUserMetadataService> userMetadataServices, IAvatarStorage avatarStorage, RbkAuthenticationOptions options)
        {
            _options = options;
            _usersService = usersService;
            _avatarStorage = avatarStorage;
            _userMetadataService = userMetadataServices;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            string avatarUrl = request.Picture;
            string avatarBase64 = null;

            if (String.IsNullOrEmpty(request.Picture))
            {
                avatarBase64 = AvatarGenerator.GenerateBase64Avatar(request.DisplayName);
            }

            if (!String.IsNullOrEmpty(request.Picture) && !request.Picture.ToLower().StartsWith("http"))
            {
                var filename = await _avatarStorage.SaveAsync(avatarBase64, _options._userAvatarPath, Guid.NewGuid().ToString("N"), cancellation);
                avatarUrl = _avatarStorage.GetRelativePath(filename);
            }

            var user = await _usersService.CreateUserAsync(request.Identity.Tenant, request.Username, request.Password, request.Email, request.DisplayName,
               avatarUrl, true, request.Metadata, cancellation);

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

            await _usersService.AppendUserMetadata(request.Username, request.Identity.Tenant, allMetadata, cancellation);

            user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellation);

            return CommandResponse.Success(user);
        }
    }
}
