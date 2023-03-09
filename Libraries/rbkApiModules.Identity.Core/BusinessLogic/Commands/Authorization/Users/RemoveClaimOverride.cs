﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class RemoveClaimOverride
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Username { get; set; }
        public Guid[] ClaimIds { get; set; }
    }

    public class Validator : AbstractValidator<Request>
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
                    RuleFor(a => a.ClaimIds)
                        .Must(HaveAtLeastOneItem).WithMessage(localization.GetValue("The list of claims must have at least one item"))
                        .DependentRules(() =>
                        {
                            RuleForEach(x => x.ClaimIds)
                                .ClaimExistOnDatabase(claimsService, localization)
                                .MustAsync(ClaimIsOverrideInUser).WithMessage(localization.GetValue("Claim is not overrided in the user"))
                                .WithName(localization.GetValue("Claim"));
                        });
                });
        }

        private async Task<bool> ClaimIsOverrideInUser(Request request, Guid claimId, CancellationToken cancellation)
        {
            var user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Identity.Tenant);

            return user.Claims.Any(x => x.ClaimId == claimId);
        }

        private async Task<bool> UserExistInDatabaseUnderTheSameTenant(Request request, string username, CancellationToken cancelation)
        {
            return await _authService.FindUserAsync(username, request.Identity.Tenant, cancelation) != null;
        }

        private bool HaveAtLeastOneItem(Guid[] claimIds)
        {
            return claimIds != null && claimIds.Any();
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IAuthService _authService;
        private readonly IClaimsService _claimsService;

        public Handler(IAuthService authService, IClaimsService claimsService)
        {
            _authService = authService;
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _claimsService.RemoveClaimOverridesAsync(request.ClaimIds, request.Username, request.Identity.Tenant, cancellation);

            var user = await _authService.GetUserWithDependenciesAsync(request.Username, request.Identity.Tenant, cancellation);

            return CommandResponse.Success(user);
        }
    }
}