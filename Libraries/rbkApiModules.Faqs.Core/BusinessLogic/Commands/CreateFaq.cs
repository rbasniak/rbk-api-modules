using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;
using rbkApiModules.Commons.Core.Utilities.Localization;

namespace rbkApiModules.Faqs.Core;

public class CreateFaq
{
    public class Request : AuthenticatedRequest, IRequest<CommandResponse>
    {
        public string Tag { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class Validator : AbstractValidator<Request>
    {
        public Validator(ILocalizationService localization)
        {
            RuleFor(x => x.Tag)
                .IsRequired(localization)
                .WithName(localization.GetValue(FaqMessages.Fields.Tag));

            RuleFor(x => x.Question)
                .IsRequired(localization)
                .WithName(localization.GetValue(FaqMessages.Fields.Question));

            RuleFor(x => x.Answer)
                .IsRequired(localization)
                .WithName(localization.GetValue(FaqMessages.Fields.Answer));
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
            var faq = await _faqsService.CreateAsync(request.Identity.Tenant, request.Tag.ToLower(), request.Question, request.Answer, cancellation);

            return CommandResponse.Success(faq);
        }
    }
}