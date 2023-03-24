using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;
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

        public Validator(IAuthService userService, ILocalizationService localization, IEnumerable<ICustomLoginPolicyValidator> loginPolicies, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;

            RuleFor(x => x)
                .LoginPoliciesAreValid(loginPolicies, localization).DependentRules(() => 
                {
                    RuleFor(x => x.Username)
                        .IsRequired(localization)
                        .MustAsync(ExistOnDatabase)
                            .WithMessage(localization.GetValue(AuthenticationMessages.Validations.InvalidCredentials))
                        .WithName(localization.GetValue(AuthenticationMessages.Fields.User))
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Username)
                                .MustAsync(BeConfirmed)
                                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.UserNotYetConfirmed));

                            RuleFor(x => x.Password)
                                .IsRequired(localization)
                                .MustAsync(MatchPassword)
                                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.InvalidCredentials))
                                .WithName(localization.GetValue(AuthenticationMessages.Fields.Password));
                        });
                });
        } 

        public async Task<bool> ExistOnDatabase(Request request, string username, CancellationToken cancellation)
        {
            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user != null;
        }

        public async Task<bool> BeConfirmed(Request request, string username, CancellationToken cancellation)
        {
            var user = await _userService.FindUserAsync(username, request.Tenant, cancellation);

            return user.IsConfirmed;
        }


        public async Task<bool> MatchPassword(Request request, string password, CancellationToken cancellation)
        {
            if (request.AuthenticationMode == AuthenticationMode.Windows)
            {
                return true;
            }
            else
            {
                var user = await _userService.FindUserAsync(request.Username, request.Tenant, cancellation);

                return PasswordHasher.VerifyPassword(password, user.Password);
            }
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _authService;
        private readonly IJwtFactory _jwtFactory;
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly IEnumerable<ICustomClaimHandler> _claimHandlers;

        public Handler(IJwtFactory jwtFactory, IOptions<JwtIssuerOptions> jwtOptions, IAuthService authService, IEnumerable<ICustomClaimHandler> claimHandlers)
        {
            _authService = authService;
            _jwtFactory = jwtFactory;
            _jwtOptions = jwtOptions.Value;
            _claimHandlers = claimHandlers;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var user = await _authService.FindUserAsync(request.Username, request.Tenant, cancellation);

            if (user.RefreshTokenValidity < DateTime.UtcNow)
            {
                var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

                await _authService.UpdateRefreshTokenAsync(request.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife, cancellation);
            }

            user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Tenant, cancellation);

            var extraClaims = new Dictionary<string, string[]>
            {
                { "authentication-mode", new[] { request.AuthenticationMode.ToString().ToLower() } }
            };

            foreach (var claimHandler in _claimHandlers)
            {
                foreach (var claim in await claimHandler.GetClaims(user.TenantId, user.Username))
                {
                    extraClaims.Add(claim.Type, new[] { claim.Value });
                }
            }

            var jwt = TokenGenerator.Generate(_jwtFactory, user, extraClaims);

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