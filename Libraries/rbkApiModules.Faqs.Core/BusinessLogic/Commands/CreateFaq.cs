using FluentValidation;
using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Core.Localization;

namespace rbkApiModules.Faqs.Core;

public class CreateFaq
{
    public class Command : AuthenticatedCommand, IRequest<CommandResponse>
    {
        public string Tag { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }

    public class Validator : AbstractValidator<Command>
    {
        public Validator(ILocalizationService localization)
        {
            RuleFor(x => x.Tag)
                .IsRequired(localization)
                .WithName(localization.GetValue("Tag"));

            RuleFor(x => x.Question)
                .IsRequired(localization)
                .WithName(localization.GetValue("Question"));

            RuleFor(x => x.Answer)
                .IsRequired(localization)
                .WithName(localization.GetValue("Answer"));
        }
    }

    public class Handler : IRequestHandler<Command, CommandResponse>
    {
        private readonly IFaqsService _faqsService;

        public Handler(IFaqsService faqsService)
        {
            _faqsService = faqsService;
        }

        public async Task<CommandResponse> Handle(Command request, CancellationToken cancellation)
        {
            var faq = await _faqsService.CreateAsync(request.Identity.Tenant, request.Tag.ToLower(), request.Question, request.Answer, cancellation);

            return CommandResponse.Success(faq);
        }
    }
}