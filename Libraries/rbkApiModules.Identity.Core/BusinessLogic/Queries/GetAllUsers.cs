using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllUsers
{
    public class Command : AuthenticatedRequest, IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IAuthService _usersService;

        public Handler(IAuthService usersService)
        {
            _usersService = usersService;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var users = await _usersService.GetAllAsync(request.Identity.Tenant);

            return QueryResponse.Success(users);
        }
    }
}