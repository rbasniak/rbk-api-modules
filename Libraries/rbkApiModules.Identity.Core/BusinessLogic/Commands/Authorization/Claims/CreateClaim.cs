using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Identity.Core;

public class CreateClaim
{
    public class Request : IRequest<CommandResponse>
    {
        public required string Identification { get; set; }
        public required string Description { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(x => x.Identification)
                .IsRequired(localization)
                .MustAsync(NotExistsInDatabaseWithSameIdentification)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimIdentificationAlreadyUsed))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Identification));

            RuleFor(x => x.Description)
                .IsRequired(localization)
                .MustAsync(NotExistsInDatabaseWithSameDescription)
                    .WithMessage(localization.LocalizeString(AuthenticationMessages.Validations.ClaimDescriptionAlreadyUsed))
                .WithName(localization.LocalizeString(AuthenticationMessages.Fields.Description));
        }

        private async Task<bool> NotExistsInDatabaseWithSameIdentification(Request request, string identification, CancellationToken cancelation)
        {
            return (await _claimsService.FindByIdentificationAsync(identification)) == null;
        }

        private async Task<bool> NotExistsInDatabaseWithSameDescription(Request request, string identification, CancellationToken cancelation)
        {
            return (await _claimsService.FindByDescriptionAsync(identification)) == null;
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            var claim = new Claim(request.Identification, request.Description);

            claim = await _claimsService.CreateAsync(claim, cancellation);

            return CommandResponse.Success(claim);
        }
    }
}