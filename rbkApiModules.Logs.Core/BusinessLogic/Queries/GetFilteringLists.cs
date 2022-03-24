using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Logs.Core
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
            private readonly ILogsModuleStore _context;

            public Handler(IHttpContextAccessor httpContextAccessor, ILogsModuleStore context)
                : base(httpContextAccessor)
            {
                _context = context;
            }

            protected override async Task<object> ExecuteAsync(Command request)
            {
                var data = new FilterOptionListData();

                var logs = await _context.AllAsync();

                data.Messages = logs.Select(x => x.Message).Distinct().OrderBy(x => x).ToList();
                data.Levels = logs.Select(x => x.Level).Distinct().OrderBy(x => x).ToList();
                data.Layers = logs.Select(x => x.ApplicationLayer).Distinct().OrderBy(x => x).ToList();
                data.Areas = logs.Select(x => x.ApplicationArea).Distinct().OrderBy(x => x).ToList();
                data.Versions = logs.Select(x => x.ApplicationVersion).Distinct().OrderBy(x => x).ToList();
                data.Sources = logs.Select(x => x.Source).Distinct().OrderBy(x => x).ToList();
                data.Enviroments = logs.Select(x => x.Enviroment).Distinct().OrderBy(x => x).ToList();
                data.EnviromentsVersions = logs.Select(x => x.EnviromentVersion).Distinct().OrderBy(x => x).ToList();
                data.Users = logs.Select(x => x.Username).Distinct().OrderBy(x => x).ToList();
                data.Domains = logs.Select(x => x.Domain).Distinct().OrderBy(x => x).ToList();
                data.Machines = logs.Select(x => x.MachineName).Distinct().OrderBy(x => x).ToList();

                return data;
            }
        }
    }
}
