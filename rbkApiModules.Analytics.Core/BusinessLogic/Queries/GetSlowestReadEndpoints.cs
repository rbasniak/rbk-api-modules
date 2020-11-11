using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Analytics.Core
{
    public class GetSlowestReadEndpoints
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
                var results = new List<SimpleLabeledValue<int>>();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo, null, null, null, null, null, null,
                    new[] { "200", "204" }, new[] { "GET" }, 0, null);

                var groupedData = data.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    results.Add(new SimpleLabeledValue<int>(itemData.Key, (int)itemData.Average(x => x.Duration)));
                }

                return results.OrderByDescending(x => x.Value).ToArray();
            }
        }
    }
}
