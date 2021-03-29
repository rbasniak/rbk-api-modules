using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class GetAllPlans
    {
        public class Command : IRequest<QueryResponse> { }

        public class Handler : BaseQueryHandler<Command>
        {
            private IWebHostEnvironment _env;
            private readonly DbContext _context;
            private readonly IMapper _mapper;

            public Handler(DbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment env)
                : base(httpContextAccessor)
            {
                _context = context;
                _mapper = mapper;
                _env = env;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var results = await _context.Set<Plan>()
                    .OrderBy(x => x.Name)
                    .Select(x => new PlanDto.Details()
                    {
                        Id = x.Id.ToString(),
                        Name = x.Name,
                        IsActive = x.IsActive,
                        Duration = x.Duration,
                        Price = x.Price,
                        IsDefault = x.IsDefault,
                        PaypalId = _env.IsDevelopment() ? x.PaypalSandboxId : x.PaypalId,
                    }).ToListAsync();

                return results;
            }
        }
    }
}
