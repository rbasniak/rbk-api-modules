using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class UserLogin
{
    public class Command : IRequest<CommandResponse>, ILoginData
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
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _userService;

        public Validator(IAuthService userService, ILocalizationService localization, IEnumerable<ICustomLoginPolicyValidator> loginPolicies, IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;

            var temp1 = httpContextAccessor.HttpContext.Request.Headers.ToList();
            var temp2 = httpContextAccessor.HttpContext.User.Identity.Name;

            RuleFor(x => x)
                .LoginPoliciesAreValid(loginPolicies, localization).DependentRules(() => 
                {
                    RuleFor(a => a.Username)
                        .IsRequired(localization)
                        .MustAsync(ExistOnDatabase).WithMessage(localization.GetValue("Invalid credentials"))
                        .WithName(localization.GetValue("User"))
                        .DependentRules(() =>
                        {
                            RuleFor(a => a.Username)
                                .MustAsync(BeConfirmed).WithMessage(localization.GetValue("User not yet confirmed"));

                            RuleFor(a => a.Password)
                                .IsRequired(localization)
                                .MustAsync(MatchPassword).WithMessage(localization.GetValue("Invalid credentials"))
                                .WithName(localization.GetValue("Senha"));
                        });
                });
        } 

        public async Task<bool> ExistOnDatabase(Command command, string username, CancellationToken cancellation)
        {
            var user = await _userService.FindUserAsync(username, command.Tenant, cancellation);

            return user != null;
        }

        public async Task<bool> BeConfirmed(Command command, string username, CancellationToken cancellation)
        {
            var user = await _userService.FindUserAsync(username, command.Tenant, cancellation);

            return user.IsConfirmed;
        }


        public async Task<bool> MatchPassword(Command command, string password, CancellationToken cancellation)
        {
            var user = await _userService.FindUserAsync(command.Username, command.Tenant, cancellation);

            return PasswordHasher.VerifyPassword(password, user.Password);
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
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

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            var user = await _authService.FindUserAsync(request.Username, request.Tenant, cancellation);

            if (user.RefreshTokenValidity < DateTime.UtcNow)
            {
                var refreshToken = Guid.NewGuid().ToString().ToLower().Replace("-", "");

                await _authService.UpdateRefreshTokenAsync(request.Username, request.Tenant, refreshToken, _jwtOptions.RefreshTokenLife);
            }

            user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Tenant);

            var extraClaims = new Dictionary<string, string[]>();

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