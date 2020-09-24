using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class GetMostActiveDomains
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

                var data = await _context.InTimeRangeAsync(request.DateFrom, request.DateTo, null, null, null, null, null, 
                    null, null, null, 0, null);

                var groupedData = data.GroupBy(x => x.Domain).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = "Without Domain";

                    if (itemData.Key != null)
                    {
                        name = itemData.Key.ToString();
                    }

                    results.Add(new SimpleLabeledValue<int>(name, itemData.Count()));
                }

                return results.OrderByDescending(x => x.Value).ToArray();
            }
        }
    }
}
