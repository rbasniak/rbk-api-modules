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
    public class GetEndpointErrorRates
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
                var results = new List<SimpleLabeledValue<double>>();

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo, null, null, null, null, null,
                    null, null, null, 0, null);

                var groupedData = data.GroupBy(x => x.Action).ToList();

                foreach (var itemData in groupedData)
                {
                    var name = itemData.Key.ToString();

                    var success = (double)itemData.Count(x => x.Response == 200 || x.Response == 204);
                    var errors = (double)itemData.Count(x => x.Response != 200 && x.Response != 204 && x.Response != 400 && x.Response != 401 && x.Response != 403);
                    
                    results.Add(new SimpleLabeledValue<double>(name, errors / (success + errors) * 100.0));
                }

                return results.OrderByDescending(x => x.Value).ToArray();
            }
        }
    }
}
