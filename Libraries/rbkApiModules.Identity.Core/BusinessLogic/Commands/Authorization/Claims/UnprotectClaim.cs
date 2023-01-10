using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class UnprotectClaim
{
    public class Command : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            RuleFor(a => a.Id)
                .ClaimExistOnDatabase(claimsService, localization);
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService claimsService)
        {
            _claimsService = claimsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            await _claimsService.UnprotectAsync(request.Id, cancellation);

            var claim = await _claimsService.FindAsync(request.Id, cancellation);

            return CommandResponse.Success(claim);
        }
    }
}