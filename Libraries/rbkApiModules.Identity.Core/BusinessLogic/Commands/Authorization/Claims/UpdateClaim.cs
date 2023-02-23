﻿using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Identity.Core;

public class UpdateClaim
{
    public class Command : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        private readonly IClaimsService _claimsService;

        public Validator(IClaimsService claimsService, ILocalizationService localization)
        {
            _claimsService = claimsService;

            RuleFor(a => a.Id)
                .ClaimExistOnDatabase(claimsService, localization);

            RuleFor(a => a.Description)
                .IsRequired(localization)
                .MustAsync(NotExistsInDatabaseWithSameDescription).WithMessage(localization.GetValue("There is already a claim with this description"))
                .WithName(localization.GetValue("Description"));
        }

        private async Task<bool> NotExistsInDatabaseWithSameDescription(Command command, string identification, CancellationToken cancelation)
        {
            return (await _claimsService.FindByDescriptionAsync(identification)) == null;
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
            await _claimsService.RenameAsync(request.Id, request.Description, cancellation);

            var claim = await _claimsService.FindAsync(request.Id, cancellation);

            return CommandResponse.Success(claim);
        }
    }
}