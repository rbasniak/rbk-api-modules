using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Threading.Tasks;

namespace rbkApiModules.Logs.Core
{
    public class DeletePriorFrom
    {
        public class Command : IRequest<CommandResponse>
        {
            public int DaysToKeep { get; set; }
        }

        public class Handler : BaseCommandHandler<Command>
        {
            private readonly ILogsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, ILogsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                _context.DeleteOldEntries(request.DaysToKeep);

                return await Task.FromResult((object)null);
            }
        }
    }
}
