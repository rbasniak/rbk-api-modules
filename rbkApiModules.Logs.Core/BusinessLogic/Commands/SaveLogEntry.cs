using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR.Core;

namespace rbkApiModules.Logs.Core
{
    public class SaveLogEntry
    {
        public class Command : IRequest<QueryResponse>
        {
            public Command() { }

            public string Message { get; set; }
            public LogLevel Level { get; set; }
            public string ApplicationLayer { get; set; }
            public string ApplicationArea { get; set; }
            public string ApplicationVersion { get; set; }
            public string InputData { get; set; }
            public string Source { get; set; }
            public string Enviroment { get; set; }
            public string EnviromentVersion { get; set; }
            public string Username { get; set; }
            public string Domain { get; set; }
            public string MachineName { get; set; }
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

            protected override Task<object> ExecuteAsync(Command request)
            {
                var entry = new LogEntry
                {
                    ApplicationLayer = request.ApplicationLayer,
                    ApplicationArea = request.ApplicationArea,
                    ApplicationVersion = request.ApplicationVersion,
                    InputData = request.InputData,
                    Source = request.Source,
                    Enviroment = request.Enviroment,
                    EnviromentVersion = request.EnviromentVersion,
                    MachineName = request.MachineName,
                    Username = request.Username,
                    Domain = request.Domain,
                    Message = request.Message,
                    Level = request.Level
                };

                _context.StoreData(entry);

                return Task.FromResult<object>(null);
            }
        }
    }
}
