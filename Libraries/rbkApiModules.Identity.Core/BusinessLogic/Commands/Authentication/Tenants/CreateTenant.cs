﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class CreateTenant
{
    public class Request : IRequest<CommandResponse>
    {
        private string _alias;

        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value?.ToUpper();
            }
        }
        public string Name { get; set; }
        public string Metadata { get; set; }

        public AdminUser AdminInfo { get; set; }
    }

    public class AdminUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly ITenantsService _tenantsService;

        public Validator(ITenantsService tenantsService, ILocalizationService localization, IEnumerable<ICustomPasswordPolicyValidator> validators)
        {
            _tenantsService = tenantsService;

            RuleFor(x => x.AdminInfo)
                .NotNull()
                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.AdminUserDataIsRequired))
                .DependentRules(() => 
                {
                    RuleFor(x => x.AdminInfo.Username)
                        .IsRequired(localization);

                    RuleFor(x => x.AdminInfo.Email)
                        .IsRequired(localization);

                    RuleFor(x => x.AdminInfo.Password)
                        .PasswordPoliciesAreValid(validators, localization);

                    RuleFor(x => x.AdminInfo.DisplayName)
                        .IsRequired(localization);

                    RuleFor(x => x.Alias)
                        .IsRequired(localization)
                        .MustAsync(NameBeUnique)
                            .WithMessage(localization.GetValue(AuthenticationMessages.Validations.TenantAliasAlreadyUsed))
                            .WithName(localization.GetValue(AuthenticationMessages.Fields.Tenant))
                        .DependentRules(() =>
                        {
                            RuleFor(x => x.Name)
                                .IsRequired(localization)
                                .MustAsync(NameNotBeingUsed)
                                    .WithMessage(localization.GetValue(AuthenticationMessages.Validations.NameAlreadyUsed))
                                    .WithName(localization.GetValue(AuthenticationMessages.Fields.Name));
                        });
                });

        }

        private async Task<bool> NameBeUnique(Request request, string alias, CancellationToken cancellation)
        {
            return await _tenantsService.FindAsync(alias, cancellation) == null;
        }

        private async Task<bool> NameNotBeingUsed(Request request, string name, CancellationToken cancellation)
        {
            var existingTenant = await _tenantsService.FindByNameAsync(name, cancellation);

            return existingTenant == null || existingTenant.Alias == request.Alias;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly ITenantsService _tenantsService;
        private readonly IClaimsService _claimsService;
        private readonly IAuthService _usersService;

        public Handler(ITenantsService tenantsService, IAuthService usersService, IClaimsService claimsService)
        {
            _tenantsService = tenantsService;
            _claimsService = claimsService;
            _usersService = usersService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var tenant = new Tenant(request.Alias, request.Name, request.Metadata);

            tenant = await _tenantsService.CreateAsync(tenant, cancellation);

            await _usersService.CreateUserAsync(
                tenant: request.Alias,
                username: request.AdminInfo.Username,
                password: request.AdminInfo.Password,
                email: request.AdminInfo.Email,
                displayName: request.AdminInfo.DisplayName,
                avatar: AvatarGenerator.GenerateBase64Avatar(request.AdminInfo.DisplayName),
                isConfirmed: true,
                cancellation
            );

            var claims = await _claimsService.GetAllAsync();

            var desiredClaims = new List<Guid>();

            desiredClaims.Add(claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USERS).Id);
            desiredClaims.Add(claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USER_ROLES).Id);
            desiredClaims.Add(claims.First(x => x.Identification == AuthenticationClaims.OVERRIDE_USER_CLAIMS).Id);
            desiredClaims.Add(claims.First(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES).Id);

            await _claimsService.AddClaimOverridesAsync(desiredClaims.ToArray(), request.AdminInfo.Username, tenant.Alias, ClaimAccessType.Allow, cancellation);

            return CommandResponse.Success(tenant);
        }
    }
}