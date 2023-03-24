using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using System.Text.Json.Serialization;

namespace rbkApiModules.Identity.Core;

public class CreateUser
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>, IUserMetadata
    {
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public Guid[] RoleIds { get; set; }
        public Dictionary<string, string> Metadata { get; set; }

        [JsonIgnore]
        public AuthenticationMode AuthenticationMode { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly IRolesService _rolesService;

        public Validator(IAuthService usersService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> passwordValidators,
            IEnumerable<ICustomUserMetadataValidator> metadataValidators, IRolesService rolesService)
        {
            _usersService = usersService;
            _rolesService = rolesService;

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserDoesNotExistOnDatabase)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.UserAlreadyExists))
                .WithName(localization.GetValue(AuthenticationMessages.Fields.User));

            RuleFor(x => x.Email)
                .IsRequired(localization)
                .MustBeEmail(localization)
                .MustAsync(EmailDoesNotExistOnDatabase)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.EmailAlreadyUsed))
                .WithName(localization.GetValue(AuthenticationMessages.Fields.Email));

            RuleFor(x => x.DisplayName)
                .IsRequired(localization)
                .WithName(localization.GetValue(AuthenticationMessages.Fields.DisplayName));

            RuleFor(x => x)
                .UserMetadataIsValid(metadataValidators, localization);

            RuleFor(x => x.Password)
                .Must(PasswordsIsRequired)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.PasswordIsRequired))
                .Must(PasswordsBeTheSame)
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.PasswordsMustBeTheSame))
                .PasswordPoliciesAreValid(passwordValidators, localization);

            RuleFor(x => x.RoleIds)
                .NotNull()
                .WithMessage(localization.GetValue(AuthenticationMessages.Validations.RoleListMustNotBeEmpty))
                .DependentRules(() =>
                {
                    RuleForEach(x => x.RoleIds)
                        .MustAsync(RoleExistOnDatabase)
                        .WithMessage(localization.GetValue(AuthenticationMessages.Validations.RoleNotFound))
                        .WithName(localization.GetValue(AuthenticationMessages.Fields.Role));
                });
        }

        private async Task<bool> EmailDoesNotExistOnDatabase(Request request, string email, CancellationToken cancellation)
        {
            return await _usersService.IsUserRegisteredAsync(email, request.Identity.Tenant, cancellation);
        } 

        private async Task<bool> RoleExistOnDatabase(Request request, Guid roleId, CancellationToken cancellation)
        {
            var role = await _rolesService.FindAsync(roleId, cancellation);

            return role != null;
        }

        private bool PasswordsBeTheSame(Request request, string _)
        {
            return request.AuthenticationMode == AuthenticationMode.Windows ? true : request.PasswordConfirmation == request.Password;
        }

        private bool PasswordsIsRequired(Request request, string _)
        {
            return request.AuthenticationMode == AuthenticationMode.Windows ? true : request.PasswordConfirmation == request.Password;
        } 

        private async Task<bool> UserDoesNotExistOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(username, request.Identity.Tenant, cancellation);

            return user != null;
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
            var password = request.Password;

            if (password == null)
            {
                password = Guid.NewGuid().ToString("N");
            }

            var avatar = AvatarGenerator.GenerateBase64Avatar(request.DisplayName);

            var filename = await _avatarStorage.SaveAsync(avatar, _options._userAvatarPath, Guid.NewGuid().ToString("N"), cancellation);
            var avatarUrl = _avatarStorage.GetRelativePath(filename); 

            await _usersService.CreateUserAsync(request.Identity.Tenant, request.Username, password, request.Email, request.DisplayName,
               avatarUrl, true, cancellation);

            var user = await _usersService.ReplaceRoles(request.Username, request.Identity.Tenant, request.RoleIds, cancellation);

            foreach (var metadataService in _userMetadataService)
            {
                await metadataService.AppendMetadataAsync(user);
            }

            return CommandResponse.Success(user);
        }
    }
}
