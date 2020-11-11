using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
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
            private readonly IDiagnosticsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, IDiagnosticsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var data = new FilterOptionListData();

                var analytics = await _context.AllAsync();

                data.Agents = analytics.Select(x => x.ClientUserAgent).Distinct().OrderBy(x => x).ToList();
                data.Areas = analytics.Select(x => x.ApplicationArea).Distinct().OrderBy(x => x).ToList();
                data.Browsers = analytics.Select(x => x.ClientBrowser).Distinct().OrderBy(x => x).ToList();
                data.Devices = analytics.Select(x => x.ClientDevice).Distinct().OrderBy(x => x).ToList();
                data.Domains = analytics.Select(x => x.Domain).Distinct().OrderBy(x => x).ToList();
                data.Layers = analytics.Select(x => x.ApplicationLayer).Distinct().OrderBy(x => x).ToList();
                data.Messages = analytics.Select(x => x.ExceptionMessage).Distinct().OrderBy(x => x).ToList();
                data.OperatinSystems = analytics.Select(x => x.ClientOperatingSystem + " " + x.ClientOperatingSystemVersion).Distinct().OrderBy(x => x).ToList();
                data.Sources = analytics.Select(x => x.ExceptionSource).Distinct().OrderBy(x => x).ToList();
                data.Users = analytics.Select(x => x.Username).Distinct().OrderBy(x => x).ToList();
                data.Versions = analytics.Select(x => x.ApplicationVersion).Distinct().OrderBy(x => x).ToList();

                return data;
            }
        }
    }
}
