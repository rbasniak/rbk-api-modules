using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllTenants
{
    public class Request : IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly ITenantsService _tenantsService;

        public Handler(ITenantsService tenantsService)
        {
            _tenantsService = tenantsService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var results = await _tenantsService.GetAllAsync();

            return QueryResponse.Success(results);
        }
    }
}
