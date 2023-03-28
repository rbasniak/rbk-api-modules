using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Faqs.Core;

public class UpdateFaq
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(ILocalizationService localization, IFaqsService faqsService)
        {
            RuleFor(x => x.Id)
                .MustAsync(async (command, id, cancellation) => await faqsService.ExistsAsync(command.Identity.Tenant, id, cancellation))
                    .WithMessage(localization.LocalizeString(FaqMessages.Validation.EntityNotFound));

            RuleFor(x => x.Question)
                .IsRequired(localization)
                .WithName(localization.LocalizeString(FaqMessages.Fields.Question));

            RuleFor(x => x.Answer)
                .IsRequired(localization)
                .WithName(localization.LocalizeString(FaqMessages.Fields.Answer));
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
            var faq = await _faqsService.UpdateAsync(request.Identity.Tenant, request.Id, request.Question, request.Answer, cancellation);

            return CommandResponse.Success(faq);
        }
    }
}