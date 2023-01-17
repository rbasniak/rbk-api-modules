using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class GetStateChangedEvents
{
    public class Command : IRequest<QueryResponse>
    {
        public Guid Id { get; set; }
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        protected readonly IStatesService _statesService;

        public Handler(IStatesService statesService)
        {
            _statesService = statesService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var results = await _statesService.GetEntityHistory(request.Id, cancellation);

            return QueryResponse.Success(results);
        } 
    }
}