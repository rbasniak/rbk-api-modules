using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
using Serilog;
using System.Text.Json.Serialization;

namespace rbkApiModules.Identity.Core;

public class UserLogin
{
    public class Request : IRequest<CommandResponse>, ILoginData
    {
        private string _tenant;

        public string Username { get; set; }
        public string Password { get; set; }
        public string Tenant
        {
            get => _tenant;
            set
            {
                _tenant = value?.ToUpper();
            }
        }

        [JsonIgnore]
        public AuthenticationMode AuthenticationMode { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IAuthService _userService;
        private readonly RbkAuthenticationOptions _authOptions;

        public Validator(IAuthService userService, ILocalizationService localization, IEnumerable<ICustomLoginPolicyValidator> loginPolicies, RbkAuthenticationOptions authOptions)
        {
            _userService = userService;
            _authOptions = authOptions;

            RuleFor(x => x)
                .LoginPoliciesAreValid(loginPolicies, localization).DependentRules(() => 
                {
                    RuleFor(x => x.Username)
                        .IsRequired(localization)
                        .MustAsync(ExistOnDatabase)
                            .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidCredentials))
                        .WithName(localization.LocalizeString(AuthenticationMessages.Fields.User))
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Username)
                                .MustAsync(BeActive)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.AccountDeactivated))
                                .MustAsync(UseCorrectAuthenticationMode)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidLoginMode))
                                .MustAsync(BeConfirmed)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.UserNotYetConfirmed));

                            RuleFor(x => x.Password)
                                .Must(IsRequiredWhenUsingCredentials)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.PasswordIsRequired))
                                .MustAsync(MatchPassword)
                                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.InvalidCredentials))
                                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Password));
                        });
                });
        }

        private bool IsRequiredWhenUsingCredentials(Request request, string password)
        {
            if (request.AuthenticationMode == AuthenticationMode.Windows)
            {
                return true;
            }   
            else if (request.AuthenticationMode == AuthenticationMode.Credentials)
            {
                return !String.IsNullOrEmpty(password);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<bool> ExistOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication;

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user != null || userWillBeAutomaticallyCreated;
        }

        public async Task<bool> BeConfirmed(Request request, string username, CancellationToken cancellation)
        {
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication;

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user != null && user.IsConfirmed || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> BeActive(Request request, string username, CancellationToken cancellation)
        {
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication;

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user != null && user.IsActive || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> UseCorrectAuthenticationMode(Request request, string username, CancellationToken cancellation)
        {
            var userWillBeAutomaticallyCreated = _authOptions._allowUserCreationOnFirstAccess && _authOptions._loginMode == LoginMode.WindowsAuthentication;

            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user != null && user.AuthenticationMode == request.AuthenticationMode || user == null && userWillBeAutomaticallyCreated;
        }

        public async Task<bool> MatchPassword(Request request, string password, CancellationToken cancellation)
        {
            if (request.AuthenticationMode == AuthenticationMode.Windows)
            {
                return true;
            }
            else if (request.AuthenticationMode == AuthenticationMode.Credentials)
            {
                var user = await _userService.FindUserAsync(request.Username, request.Tenant, cancellation);

                return PasswordHasher.VerifyPassword(password, user.Password);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;
        private readonly IAutomaticUserCreator _automaticUserCreator;

        public Handler(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IAuthService authService, 
            IEnumerable<ICustomClaimHandler> claimHandlers, IAutomaticUserCreator automaticUserCreator)
        {
            _jwtFactory = jwtFactory;
            _authService = authService;
            _jwtOptions = jwtOptions.Value;
            _claimHandlers = claimHandlers;
            _automaticUserCreator = automaticUserCreator;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _authService.FindUserAsync(request.Username, request.Tenant, cancellation);

            if (user == null)
            {
                user = await _automaticUserCreator.CreateIfAllowedAsync(request.Username, request.Tenant, cancellation);
            }

            Log.Information($"Loging in with user {user.Username}");

            if (user.RefreshTokenValidity < DateTime.UtcNow)
            {
                var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

                await _authService.UpdateRefreshTokenAsync(request.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellation);
            }

            user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Tenant, cancellation);

            var extraClaims = new Dictionary<string, string[]>
            {
                { JwtClaimIdentifiers.AuthenticationMode, new[] { user.AuthenticationMode.ToString() } }
            };

            Log.Information($"Token generated with AuthenticationMode={user.AuthenticationMode}");

            foreach (var claimHandler in _claimHandlers)
            {
                foreach (var claim in await claimHandler.GetClaims(user.TenantId, user.Username))
                {
                    extraClaims.Add(claim.Type, new[] { claim.Value });
                }
            }

            var jwt = await TokenGenerator.GenerateAsync(_jwtFactory, user, extraClaims);

            await _authService.RefreshLastLogin(request.Username, request.Tenant, cancellation);

            return CommandResponse.Success(jwt);
        }
    }
}

public enum AuthenticationMode
{
    Credentials,
    Windows
}

