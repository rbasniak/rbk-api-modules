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
    public class GetFilteringLists
    {
        public class Command : IRequest<QueryResponse>
        {
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
                var data = new FilterOptionListData();

                var analytics = await _context.AllAsync();

                data.Actions = analytics.Select(x => x.Action).Distinct().ToList();
                data.Agents = analytics.Select(x => x.UserAgent).Distinct().ToList();
                data.Areas = analytics.Select(x => x.Area).Distinct().ToList();
                data.Domains = analytics.Select(x => x.Domain).Distinct().ToList();
                data.Methods = analytics.Select(x => x.Method).Distinct().ToList();
                data.Responses = analytics.Select(x => x.Response.ToString()).Distinct().ToList();
                data.Users = analytics.Select(x => x.Username).Distinct().ToList();

                return data;
            }
        }
    }
}
