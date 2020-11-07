using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.MediatR.Core;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    public class GetDailyLayerErrors
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            }
            public Command(DateTime from, DateTime to)
            {
                DateFrom = from;
                DateTo = to;
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                CascadeMode = CascadeMode.Stop;
            }
        }

        public class Handler : BaseQueryHandler<Command>
        {
            private readonly IDiagnosticsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IDiagnosticsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                return await ChartUtils.BuildChartData(_context, request.DateFrom, request.DateTo, x => x.ApplicationLayer, entry => entry.ApplicationLayer);
            }
        }
    }
}
