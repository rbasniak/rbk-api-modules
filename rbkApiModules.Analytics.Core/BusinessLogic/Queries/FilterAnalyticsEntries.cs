using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Analytics.Core
{
    public class FilterAnalyticsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Areas = new string[0];
                Domains = new string[0];
                Agents = new string[0];
                Actions = new string[0];
                Responses = new string[0];
                Users = new string[0];
                Versions = new string[0];
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string[] Areas { get; set; }
            public string[] Domains { get; set; }
            public string[] Actions { get; set; }
            public string[] Responses { get; set; }
            public string[] Users { get; set; }
            public string[] Agents { get; set; }
            public string[] Versions { get; set; }
            public int Duration { get; set; }
            public string EntityId { get; set; }
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
                return await _context.FilterAsync(request.DateFrom, request.DateTo, request.Versions, request.Areas, request.Domains, request.Actions,
                    request.Users, request.Agents, request.Responses, null, request.Duration, request.EntityId);
            }
        }
    }
}
