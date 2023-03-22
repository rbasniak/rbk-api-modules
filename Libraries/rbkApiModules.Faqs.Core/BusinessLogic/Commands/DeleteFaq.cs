using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Faqs.Core;

public class DeleteFaq
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(IFaqsService faqsService, ILocalizationService localization) 
        {
            RuleFor(x => x.Id)
                .MustAsync(async (command, id, cancellation) => await faqsService.ExistsAsync(command.Identity.Tenant, id, cancellation))
                    .WithMessage(localization.GetValue(FaqMessages.Validation.EntityNotFound));
        }
    }

    public class Handler : IRequestHandler<Request, CommandResponse>
    {
        private readonly IFaqsService _faqsService;

        public Handler(IFaqsService faqsService)
        {
            _faqsService = faqsService;
        }

        public async Task<CommandResponse> Handle(Request request, CancellationToken cancellation)
        {
            await _faqsService.DeleteAsync(request.Identity.Tenant, request.Id, cancellation);

            return CommandResponse.Success();
        }
    }
}