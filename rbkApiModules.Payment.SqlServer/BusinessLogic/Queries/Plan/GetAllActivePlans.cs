using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class GetAllActivePlans
    {
        public class Command : IRequest<QueryResponse> { }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly DbContext _context;
            private readonly IMapper _mapper;

            public Handler(DbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
            {
                _context = context;
                _mapper = mapper;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = await _context.Set<Plan>()
                    .OrderBy(x => x.Name)
                    .Where(x => x.IsActive)
                    .ProjectTo<PlanDto.Details>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                return results;
            }
        }
    }
}
