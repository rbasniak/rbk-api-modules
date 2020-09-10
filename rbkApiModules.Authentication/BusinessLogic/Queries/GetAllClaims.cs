using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.Models;
using System.Linq;
using System.Threading.Tasks;

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

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                CascadeMode = CascadeMode.Stop;
            }
        }

        public class Handler : BaseQueryHandler<Command, DbContext>
        {
            private readonly IMapper _mapper;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor)
            {
                _mapper = mapper;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = await _context.Set<Claim>()
                    .OrderBy(x => x.Name)
                    .ProjectTo<SimpleNamedEntity>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return results;
            }
        }
    }
}
