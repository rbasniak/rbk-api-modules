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
    public class GetCurrentSessions
    {
        public class Command : IRequest<CommandResponse>
        {
        }

        public class Handler : BaseCommandHandler<Command>
        {
            public Handler(IHttpContextAccessor httpContextAccessor)
                : base(httpContextAccessor)
            {
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                return await Task.FromResult(new 
                { 
                    Sessions = SessionAnalyticsMiddleware.Sessions,
                    Log = SessionWriter.Log
                });
            }
        }
    }
}
