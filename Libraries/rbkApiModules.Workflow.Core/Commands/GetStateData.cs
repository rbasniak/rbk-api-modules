using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class GetStateData
{
    public class Request : IRequest<QueryResponse>
    {
    } 

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IStatesService _statesService;

        public Handler(IStatesService statesService)
        {
            _statesService = statesService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var result = await _statesService.GetGroups(cancellation);

            return QueryResponse.Success(result);
        }  
    }
}
