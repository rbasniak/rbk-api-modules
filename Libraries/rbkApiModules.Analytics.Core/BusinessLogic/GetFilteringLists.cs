using MediatR;
using rbkApiModules.Commons.Core;
using rbkApiModules.Commons.Localization;

namespace rbkApiModules.Commons.Analytics;

public class GetFilteringLists
{
    public class Request : IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IAnalyticModuleStore _context;

        public Handler(IAnalyticModuleStore context)
        {
            _context = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            return QueryResponse.Success(await _context.GetFilteringLists());
        }
    }
}