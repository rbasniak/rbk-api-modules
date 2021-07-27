using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Diagnostics.Core
{
    public class DeletePriorFrom
    {
        public class Command : IRequest<CommandResponse>
        {
            public int DaysToKeep { get; set; }
        }

        public class Handler : BaseCommandHandler<Command>
        {
            private readonly IDiagnosticsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IDiagnosticsModuleStore context)
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
