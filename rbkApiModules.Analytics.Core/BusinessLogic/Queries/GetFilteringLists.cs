using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using System.Linq;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

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

                data.Actions = analytics.Select(x => x.Action).Distinct().OrderBy(x => x).ToList();
                data.Agents = analytics.Select(x => x.UserAgent).Distinct().OrderBy(x => x).ToList();
                data.Areas = analytics.Select(x => x.Area).Distinct().OrderBy(x => x).ToList();
                data.Domains = analytics.Select(x => x.Domain).Distinct().OrderBy(x => x).ToList();
                data.Responses = analytics.Select(x => x.Response.ToString()).Distinct().OrderBy(x => x).ToList();
                data.Users = analytics.Select(x => x.Username).Distinct().OrderBy(x => x).ToList();
                data.Versions = analytics.Select(x => x.Version).Distinct().OrderBy(x => x).ToList();

                return data;
            }
        }
    }
}
