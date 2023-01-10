using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllRoles
{
    public class Command : AuthenticatedCommand, IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Command, QueryResponse>
    {
        private readonly IRolesService _rolesService;

        public Handler(IRolesService context)
        {
            _rolesService = context;
        }

        public async Task<QueryResponse> Handle(Command request, CancellationToken cancellation)
        {
            var roles = await _rolesService.GetAllAsync();

            List<Role> results;

            if (request.Identity.HasTenant)
            {
                results = roles.Where(x => x.HasTenant).ToList();

                foreach (var role in roles.Where(x => x.HasNoTenant))
                {
                    if (!results.Any(x => x.Name.ToUpper() == role.Name.ToUpper()))
                    {
                        results.Add(role);
                    }
                }

                results = results.OrderBy(x => x.Name).ToList();
            }
            else
            {
                results = roles.Where(x => x.HasNoTenant).ToList();
            }

            return QueryResponse.Success(results);
        }
    }
}