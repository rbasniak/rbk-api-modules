using FluentValidation;
using MediatR;
using Microsoft.IdentityModel.Tokens;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class ChangePassword
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string PasswordConfirmation { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> passwordValidators)
        {
            _usersService = usersService;

            RuleFor(x => x.OldPassword)
                .MustAsync(MatchPassword)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.OldPasswordDoesNotMatch));

            RuleFor(x => x.NewPassword)
                .IsRequired(localization)
                .Must(PasswordsBeTheSame)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordsMustBeTheSame))
                .PasswordPoliciesAreValid(passwordValidators, localization);
        }

        private bool PasswordsBeTheSame(Request request, string _)
        {
            return request.PasswordConfirmation == request.NewPassword;
        }

        public async Task<bool> MatchPassword(Request request, string password, CancellationToken cancellation)
        {
            var user = await _usersService.FindUserAsync(request.Identity.Username, request.Identity.Tenant, cancellation);

            return PasswordHasher.VerifyPassword(password, user.Password);
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _usersService.ChangePasswordAsync(request.Identity.Tenant, request.Identity.Username, request.NewPassword, cancellation);

            return CommandResponse.Success();
        }
    }
}
