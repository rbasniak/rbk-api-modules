using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Diagnostics.Commons;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Diagnostics.Core
{
    public class SaveDiagnosticsEntry
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command()
            {

            }

            public string StackTrace { get; set; }

            public string InputData { get; set; }

            public string ApplicationArea { get; set; }

            public string ApplicationVersion { get; set; }

            public string ApplicationLayer { get; set; }

            public string DatabaseExceptions { get; set; }
            
            public string ExceptionMessage { get; set; }

            public string Username { get; set; }

            public string Domain { get; set; }

            public string ExceptionSource { get; set; }

            public string ClientBrowser { get; set; }

            public string ClientUserAgent { get; set; }

            public string ClientOperatingSystem { get; set; }

            public string ClientOperatingSystemVersion { get; set; }

            public string ClientDevice { get; set; }
            
            public string ExtraData { get; set; }
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

            protected override Task<object> ExecuteAsync(Command request)
            {
                var data = new DiagnosticsEntry();

                data.ApplicationArea = request.ApplicationArea;
                data.ApplicationLayer = request.ApplicationLayer;
                data.ApplicationVersion = request.ApplicationVersion;
                data.ClientBrowser = request.ClientBrowser;
                data.DatabaseExceptions = request.DatabaseExceptions;
                data.ClientDevice = request.ClientDevice;
                data.Domain = request.Domain;
                data.StackTrace = request.StackTrace;
                data.InputData = request.InputData;
                data.ExceptionMessage = request.ExceptionMessage;
                data.ClientOperatingSystem = request.ClientOperatingSystem;
                data.ClientOperatingSystemVersion = request.ClientOperatingSystemVersion;
                data.ExceptionSource = request.ExceptionSource;
                data.ClientUserAgent = request.ClientUserAgent;
                data.Username = request.Username;
                data.ExtraData = request.ExtraData;

                _context.StoreData(data);

                return Task.FromResult<object>(null);
            }
        }
    }
}
