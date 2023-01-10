using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllTenants
{
    public class Command : IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly ITenantsService _tenantsService;

        public Handler(ITenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var results = await _tenantsService.GetAllAsync();

            return QueryResponse.Success(results);
        }
    }
}
