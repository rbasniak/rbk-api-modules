using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllClaims
{
    public class Command : IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IClaimsService _claimsService;

        public Handler(IClaimsService context)
        {
            _claimsService = context;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var results = await _claimsService.GetAllAsync();

            return QueryResponse.Success(results);
        }
    }
}
