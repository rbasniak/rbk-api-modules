namespace rbkApiModules.Identity.Core;

public class RedefinePassword : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/authentication/redefine-password", async (Request request, IDispatcher dispatcher, CancellationToken cancellationToken) =>
        {
            var result = await dispatcher.SendAsync(request, cancellationToken);

            return ResultsMapper.FromResponse(result);
        })
        .AllowAnonymous()
        .WithName("Redefine Password")
        .WithTags("Authentication");
    }

    public class Request : ICommand
    {
        public string Code { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.Password)
                 .NotEmpty()
                 .DependentRules(() =>
                 {
                     RuleFor(x => x.Code)
                         .NotEmpty()
                         .MustAsync(ExistOnDatabaseAndIsValid)
                         .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordResetCodeExpiredOrUsed));
                 });  
        }

        public async Task<bool> ExistOnDatabaseAndIsValid(Request request, string code, CancellationToken cancellationToken)
        {
            return await _usersService.IsPasswordResetCodeValidAsync(code, cancellationToken);
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IAuthenticationMailService _mailingService;
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService, IAuthenticationMailService mailingService)
        {
            _mailingService = mailingService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var user = await _usersService.RedefinePasswordAsync(request.Code, request.Password, cancellationToken);

            _mailingService.SendPasswordResetSuccessMail(user.DisplayName, user.Email);

            return CommandResponse.Success();
        }
    }
}