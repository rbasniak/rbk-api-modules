using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Core;
using rbkApiModules.Infrastructure.MediatR;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core
{
    public class FilterDiagnosticsEntries
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {
                Areas = new string[0];
                Domains = new string[0];
                Agents = new string[0];
                Sources = new string[0];
                Browsers = new string[0];
                Users = new string[0];
                Versions = new string[0];
                Layers = new string[0];
                OperatinSystems = new string[0];
                Devices = new string[0];
            }

            public DateTime DateFrom { get; set; }
            public DateTime DateTo { get; set; }
            public string[] Areas { get; set; }
            public string[] Layers { get; set; }
            public string[] Domains { get; set; }
            public string[] Sources { get; set; }
            public string[] Browsers { get; set; }
            public string[] Users { get; set; }
            public string[] Agents { get; set; }
            public string[] Versions { get; set; }
            public string[] OperatinSystems { get; set; }
            public string[] Devices { get; set; }
            public string MessageContains { get; set; }
            public string RequestId { get; set; }
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
                return await _context.FilterAsync(request.DateFrom, request.DateTo, request.Versions, request.Areas, request.Layers,
                    request.Domains, request.Sources, request.Users, request.Browsers, request.Agents, request.OperatinSystems, 
                    request.Devices, request.MessageContains, request.RequestId);
            }
        }
    }
}
