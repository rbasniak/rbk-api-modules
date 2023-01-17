using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class GetStateData
{
    public class Command : IRequest<QueryResponse>
    {
    } 

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IStatesService _statesService;

        public Handler(IStatesService statesService)
        {
            _statesService = statesService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var result = await _statesService.GetGroups(cancellation);

            return QueryResponse.Success(result);
        }  
    }
}
