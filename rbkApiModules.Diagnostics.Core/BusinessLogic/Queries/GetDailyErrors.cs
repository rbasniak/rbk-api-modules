using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    public class GetDailyErrors
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
                var data = await _context.FilterAsync(request.DateFrom, request.DateTo, null, null, null, null, null, null, null, null, null, null, null, null);

                var groupedData = data.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(request.DateFrom, request.DateTo);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var errors = itemData.Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = errors;
                }

                return chartData;
            }
        }
    }
}
