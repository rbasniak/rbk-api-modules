using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using rbkApiModules.Infrastructure.MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class FilterAnalyticsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Areas = new List<string>();
                Domains = new List<string>();
                Methods = new List<string>();
                Agents = new List<string>();
                Actions = new List<string>();
                Responses = new List<string>();
                Users = new List<string>();
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public List<string> Areas { get; set; }
            public List<string> Domains { get; set; }
            public List<string> Methods { get; set; }
            public List<string> Actions { get; set; }
            public List<string> Responses { get; set; }
            public List<string> Users { get; set; }
            public List<string> Agents { get; set; }
            public int Duration { get; set; }
            public string EntityId { get; set; }
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
                return await _context.InTimeRangeAsync(request.DateFrom, request.DateTo, request.Areas, request.Domains, request.Actions, 
                    request.Users, request.Agents, request.Methods, request.Responses, request.Duration, request.EntityId);
            }
        }
    }
}
