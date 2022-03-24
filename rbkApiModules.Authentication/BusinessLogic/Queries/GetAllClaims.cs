using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Authentication
{
    /// <summary>
    /// Comando para pegar uma lista de permissões de acesso
    /// </summary>
    public class GetAllClaims
    {
        public class Command : IRequest<QueryResponse>
        {
        } 

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) 
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var authenticationGroup = _httpContextAccessor.GetAuthenticationGroup();

                var results = await _context.Set<Claim>()
                    .Where(x => EF.Functions.Like(authenticationGroup, x.AuthenticationGroup))
                    .OrderBy(x => x.Name)
                    .ToListAsync();

                return results;
            }
        }
    }
}
