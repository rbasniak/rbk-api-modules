using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Faqs.Core;

public class GetFaqs
{
    public class Command : AuthenticatedRequest, IRequest<QueryResponse>
    {
        public string Tag { get; set; }
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IFaqsService _faqsService;

        public Handler(IFaqsService faqsService)
        {
            _faqsService = faqsService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var faqs = await _faqsService.GetAllAsync(request.Identity.Tenant, request.Tag.ToLower(), cancellation);

            return QueryResponse.Success(faqs);
        }
    }
}
