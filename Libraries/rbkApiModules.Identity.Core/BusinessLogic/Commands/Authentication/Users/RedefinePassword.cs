using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class RedefinePassword
{
    public class Request : IRequest<CommandResponse>
    {
        public string Code { get; set; }
        public string Password { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.Password)
                 .IsRequired(localization)
                 .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Password))
                 .DependentRules(() =>
                 {
                     RuleFor(x => x.Code)
                         .IsRequired(localization)
                         .MustAsync(ExistOnDatabaseAndIsValid).WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordResetCodeExpiredOrUsed))
                         .WithName(localization.LocalizeString(AuthenticationMessages.Fields.PasswordResetCode));
                 });  
        }

        public async Task<bool> ExistOnDatabaseAndIsValid(Request request, string code, CancellationToken cancelation)
        {
            return await _usersService.IsPasswordResetCodeValidAsync(code, cancelation);
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _usersService.RedefinePasswordAsync(request.Code, request.Password, cancellation);

            _mailingService.SendPasswordResetSuccessMail(user.DisplayName, user.Email);

            return CommandResponse.Success();
        }
    }
}