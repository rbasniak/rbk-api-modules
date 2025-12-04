namespace rbkApiModules.Identity.Core;

public class ResendEmailConfirmation : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/resend-confirmation", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .AllowAnonymous()
        .WithName("Resend Email Confirmation")
        .WithTags("Authentication");
    }

    public class Request : AuthenticatedRequest, ICommand
    {
        public string Email { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .MustAsync(EmailBeRegistered)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailNotFound))
                .MustAsync(EmailBeUnconfirmed)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailAlreadyConfirmed));
        }

        private async Task<bool> EmailBeRegistered(Request request, string email, CancellationToken cancellationToken)
        {
            return await _authService.IsUserRegisteredAsync(email, request.Identity.Tenant, cancellationToken);
        }

        private async Task<bool> EmailBeUnconfirmed(Request request, string email, CancellationToken cancellationToken)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, request.Identity.Tenant, cancellationToken);

            return !isConfirmed;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _authService;

        public Handler(IAuthService authService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _authService = authService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var user = await _authService.FindUserAsync(request.Email, request.Identity.Tenant, cancellationToken);

            _mailingService.SendConfirmationMail(user.DisplayName, user.Email, user.ActivationCode);

            return CommandResponse.Success();
        }
    }
}
