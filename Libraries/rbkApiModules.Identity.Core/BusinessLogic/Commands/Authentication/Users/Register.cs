using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class Register
{
    public class Request : IRequest<CommandResponse>, IUserMetadata
    {
        public string Tenant { get; set; }
        public string DisplayName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;
        private readonly ITenantsService _tenantsService;

        public Validator(IAuthService usersService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> passwordValidators,
            IEnumerable<ICustomUserMetadataValidator> metadataValidators, ITenantsService tenantsService)
        {
            _usersService = usersService;
            _tenantsService = tenantsService;

            RuleFor(x => x.Username)
                .IsRequired(localization)
                .MustAsync(UserDoesNotExistOnDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserAlreadyExists))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User));

            RuleFor(x => x.Tenant)
                .IsRequired(localization)
                .MustAsync(TenantExistInDatabase)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.TenantNotFound))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.TenantAlias));

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
                .IsRequired(localization)
                .Must(PasswordsBeTheSame)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordsMustBeTheSame))
                .PasswordPoliciesAreValid(passwordValidators, localization);
        }

        private async Task<bool> EmailDoesNotExistOnDatabase(Request request, string email, CancellationToken cancellation)
        {
            var isRegistered = await _usersService.IsUserRegisteredAsync(email, request.Tenant, cancellation);

            return !isRegistered;
        }

        private bool PasswordsBeTheSame(Request request, string _)
        {
            return request.PasswordConfirmation == request.Password;
        }

        private async Task<bool> TenantExistInDatabase(Request request, string tenantAlias, CancellationToken cancellation)
        {
            var tenant = await _tenantsService.FindAsync(tenantAlias, cancellation);

            return tenant != null;
        }

        private async Task<bool> UserDoesNotExistOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(username, request.Tenant, cancellation);

            return user == null;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _usersService;
        private readonly IAuthenticationMailService _mailingService;

        public Handler(IAuthService usersService, IAuthenticationMailService mailingService)
        {
            _usersService = usersService;
            _mailingService = mailingService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _usersService.CreateUserAsync(request.Tenant, request.Username, request.Password, request.Email, request.DisplayName,
                AvatarGenerator.GenerateBase64Avatar(request.DisplayName), false, request.Metadata, cancellation);

            _mailingService.SendConfirmationMail(user.DisplayName, user.Email, user.ActivationCode);

            return CommandResponse.Success();
        }
    }
}
