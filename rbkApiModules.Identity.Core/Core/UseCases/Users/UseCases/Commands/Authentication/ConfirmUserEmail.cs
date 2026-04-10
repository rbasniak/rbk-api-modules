using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core.Authentication;

namespace rbkApiModules.Identity.Core;
        
public class ConfirmUserEmail : IEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/authentication/confirm-email", async (
            [FromQuery] string? email,
            [FromQuery] string? code,
            [FromQuery] string? tenant, 
            IDispatcher dispatcher, 
            ILogger<ConfirmUserEmail> logger, 
            IOptions<AuthEmailOptions> authEmailOptionsConfig, 
            CancellationToken cancellationToken) =>
        {
            var authEmailOptions = authEmailOptionsConfig.Value;

            var result = await dispatcher.SendAsync(new Request { Email = email, ActivationCode = code, Tenant = tenant }, cancellationToken);

            if (result.IsValid)
            {
                return Results.Redirect(authEmailOptions.EmailData.ConfirmationSuccessUrl);
            }
            else
            {
                return Results.Redirect(authEmailOptions.EmailData.ConfirmationFailedUrl);
            }
        })
        .AllowAnonymous()
        .WithName("Confirm User Email")
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
        public string ActivationCode { get; set; } = string.Empty;
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _usersService;

        public Validator(IAuthService usersService, ILocalizationService localization)
        {
            _usersService = usersService;

            RuleFor(x => x.ActivationCode)
                .NotEmpty();

            RuleFor(x => x.Email)
                .NotEmpty()
                .MustAsync(BeValidPair)
                .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidActivationCode));
        }

        public async Task<bool> BeValidPair(Request request, string email, CancellationToken cancellationToken)
        {
            var user = await _usersService.FindUserAsync(email, request.Tenant, cancellationToken);

            if (user == null) return false;

            return user.ActivationCode == request.ActivationCode;
        }
    }

    public class Handler : ICommandHandler<Request>
    {
        private readonly IAuthService _usersService;
        private readonly IAuthenticationMailService _mailingService;

        public Handler(IAuthService usersService, IAuthenticationMailService mailingService)
        {
            _usersService = usersService;
            _mailingService = mailingService;
        }

        public async Task<CommandResponse> HandleAsync(Request request, CancellationToken cancellationToken)
        {
            var user = await _usersService.FindUserAsync(request.Email, request.Tenant, cancellationToken);

            await _usersService.ConfirmUserAsync(request.Email, request.Tenant, cancellationToken);

            _mailingService.SendConfirmationSuccessMail(user.DisplayName, user.Email!);

            return CommandResponse.Success();
        }
    }
}