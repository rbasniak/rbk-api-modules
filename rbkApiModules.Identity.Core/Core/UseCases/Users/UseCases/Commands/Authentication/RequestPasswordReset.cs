namespace rbkApiModules.Identity.Core;

public class RequestPasswordReset : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/reset-password", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .AllowAnonymous()
        .WithName("Request Password Reset")
        .WithTags("Authentication");
    }

    public class Request : ICommand
    {
        private string _tenant = string.Empty;

        public string Tenant
        {
            get => _tenant;
            set
            {
                _tenant = value?.ToUpper() ?? string.Empty;
            }
        }
        public string Email { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(x => x.Tenant)
                .NotEmpty()
                .DependentRules(() =>
                {
                    RuleFor(x => x.Email)
                        .NotEmpty()
                        .EmailAddress()
                        .MustAsync(EmailBeRegistered)
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailNotFound))
                        .MustAsync(EmailBeConfirmed)
                        .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.EmailNotYetConfirmed));
                });

        }

        private async Task<bool> EmailBeRegistered(Request request, string email, CancellationToken cancelation = default)
        {
            return await _authService.IsUserRegisteredAsync(email, request.Tenant, cancelation);
        }

        private async Task<bool> EmailBeConfirmed(Request request, string email, CancellationToken cancellationToken)
        {
            var isConfirmed = await _authService.IsUserConfirmedAsync(email, request.Tenant, cancellationToken);

            return isConfirmed;
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
            await _authService.RequestPasswordResetAsync(request.Email, request.Tenant, cancellationToken);

            var user = await _authService.FindUserAsync(request.Email, request.Tenant, cancellationToken);

            _mailingService.SendPasswordResetMail(user.DisplayName, user.Email, user.PasswordRedefineCode.Hash);

            return CommandResponse.Success();
        }
    }
}