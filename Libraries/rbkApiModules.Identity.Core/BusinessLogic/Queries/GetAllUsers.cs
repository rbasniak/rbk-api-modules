using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllUsers
{
    public class Request : AuthenticatedRequest, IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var users = await _usersService.GetAllAsync(request.Identity.Tenant, cancellation);

            return QueryResponse.Success(users);
        }
    }
}