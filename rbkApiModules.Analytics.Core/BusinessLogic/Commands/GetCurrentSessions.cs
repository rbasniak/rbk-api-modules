using MediatR;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using rbkApiModules.Infrastructure.MediatR.Core;

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
