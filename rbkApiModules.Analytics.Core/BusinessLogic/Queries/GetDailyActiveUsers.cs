using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class GetDailyActiveUsers
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
            private readonly IAnalyticModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IAnalyticModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var data = await _context.FilterAsync(request.DateFrom, request.DateTo, null, null, null, null, null,
                    null, null, null, 0, null);

                var groupedData = data.GroupBy(x => x.Timestamp.Date).ToList();

                var chartData = ChartingUtilities.BuildLineChartAxis(request.DateFrom, request.DateTo);

                foreach (var itemData in groupedData)
                {
                    var date = itemData.Key;
                    var activeUsers = itemData.GroupBy(x => x.Username).Count();

                    var point = chartData.First(x => x.Date == date);

                    point.Value = activeUsers;
                }

                return chartData;
            }
        }
    }
}
