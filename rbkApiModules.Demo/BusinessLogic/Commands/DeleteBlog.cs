using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.MediatR.Core;
using System.Diagnostics;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class DeleteBlog
    {
        public class Command : IRequest<CommandResponse>
        {
            [ExistingEntity(typeof(Blog)), NonUsedEntity]
            public Guid Id { get; set; }
        } 

        public class Handler : BaseCommandHandler<Command, DatabaseContext>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                Debugger.Break();

                return (null, null);
            }
        }
    }
}
