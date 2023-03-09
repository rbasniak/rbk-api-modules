using AutoMapper.Configuration.Annotations;
using MediatR;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Identity.Core;

public class GetAllRoles
{
    public class Request : AuthenticatedRequest, IRequest<QueryResponse>
    {
    }

    public class Handler : IRequestHandler<Request, QueryResponse>
    {
        private readonly IRolesService _rolesService;

        public Handler(IRolesService context)
        {
            _rolesService = context;
        }

        public async Task<QueryResponse> Handle(Request request, CancellationToken cancellation)
        {
            var roles = await _rolesService.GetAllAsync();

            List<Role> results;

            if (request.Identity.HasTenant)
            {
                results = roles.Where(x => x.HasTenant).ToList();

                foreach (var role in roles.Where(x => x.HasNoTenant))
                {
                    var roleAlreadyInResults = results.FirstOrDefault(x => x.Name.ToUpper() == role.Name.ToUpper());

                    if (roleAlreadyInResults == null)
                    {
                        role.SetMode(isOverwritten: false);
                        results.Add(role);
                    }
                    else
                    {
                        role.SetMode(isOverwritten: true);
                    }
                }

                results = results.OrderBy(x => x.Name).ToList();
            }
            else
            {
                results = roles.Where(x => x.HasNoTenant).ToList();

                foreach (var role in results)
                {
                    role.SetMode(isOverwritten: false);
                }
            }

            return QueryResponse.Success(results);
        }
    }
}