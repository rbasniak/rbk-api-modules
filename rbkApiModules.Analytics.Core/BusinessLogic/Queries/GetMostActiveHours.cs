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
    public class GetMostActiveHours
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

                var data = await _context.FilterAsync(request.DateFrom, request.DateTo, null, null, null, null, null,
                    null, null, null, 0, null);

                var groupedData = data.GroupBy(x => x.Timestamp.Hour).ToList();

                for (int i = 0; i < 24; i++)
                {
                    results.Add(new SimpleLabeledValue<int>(i.ToString("00"), 0));
                }

                foreach (var itemData in groupedData)
                {
                    var element = results.First(x => x.Label == itemData.Key.ToString("00"));
                    element.Value = itemData.Count();
                }

                return results.ToArray();
            }
        }
    }
}
