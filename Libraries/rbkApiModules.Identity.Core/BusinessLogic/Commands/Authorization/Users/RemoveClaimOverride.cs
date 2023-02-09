﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class RemoveClaimOverride
{
    public class Command : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
        public Guid ClaimId { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IAuthService _authService;

        public Validator(IAuthService authService, IClaimsService claimsService, ILocalizationService localization)
        {
            _authService = authService;

            RuleFor(a => a.Username)
                .IsRequired(localization)
                .MustAsync(UserExistInDatabaseUnderTheSameTenant).WithMessage(localization.GetValue("User not found."))
                .WithName(localization.GetValue("User"))
                .DependentRules(() =>
                {
                    RuleFor(a => a.ClaimId)
                        .ClaimExistOnDatabase(claimsService, localization)
                        .MustAsync(ClaimIsOverrideInUser).WithMessage(localization.GetValue("Claim is not overrided in the user"))
                        .WithName(localization.GetValue("Claim"));
                });
        }

        private async Task<bool> ClaimIsOverrideInUser(Command command, Guid claimId, CancellationToken cancellation)
        {
            var user = await _authService.GetUserWithDependenciesAsync(command.Username, command.Identity.Tenant);

            return user.Claims.Any(x => x.ClaimId == claimId);
        }

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Command command, string username, CancellationToken cancelation)
        {
            return await _authService.FindUserAsync(username, command.Identity.Tenant, cancelation) != null;
        } 
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IAuthService _authService;
        private readonly IClaimsService _claimsService;

        public Handler(IAuthService authService, IClaimsService claimsService)
        {
            _authService = authService;
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _claimsService.RemoveClaimOverrideAsync(request.ClaimId, request.Username, request.Identity.Tenant, cancellation);

            var user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Identity.Tenant, cancellation);

            return CommandResponse.Success(user.Claims);
        }
    }
}