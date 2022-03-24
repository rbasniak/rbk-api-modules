using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Analytics.Core
{
    public class NormalizePathsAndActions
    {
        public class Command : IRequest<CommandResponse>
        {
        }

        public class Handler : BaseCommandHandler<Command>
        {
            private readonly IAnalyticModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IAnalyticModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                 _context.NormalizePathsAndActions();

                return await Task.FromResult((object)null);
            }
        }
    }
}
