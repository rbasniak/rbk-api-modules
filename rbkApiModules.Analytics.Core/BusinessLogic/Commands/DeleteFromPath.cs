using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Utilities;
using rbkApiModules.Utilities.Charts.ChartJs;
using rbkApiModules.Utilities.Charts;
using rbkApiModules.Diagnostics.Commons;

namespace rbkApiModules.Analytics.Core
{
    public class DeleteFromPath
    {
        public class Command : IRequest<CommandResponse>
        {
            public string SearchText { get; set; }
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
                _context.DeleteStatisticsFromMatchingPathAsync(request.SearchText);

                return await Task.FromResult((object)null);
            }
        }
    }
}
