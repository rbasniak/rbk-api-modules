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

namespace rbkApiModules.Demo.BusinessLogic
{
    public class TestCommand
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Data1 { get; set; }
            public string Data2 { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DatabaseContext _context;

            public Validator(DatabaseContext context)
            {
                _context = context;

                RuleFor(x => x.Data1)
                    .Must(MustTest).WithMessage("Testeeee");
            }

            private bool MustTest(Command command, string test)
            {
                return false;
            } 
        }

        public class Handler : BaseCommandHandler<Command, DatabaseContext>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                 

                return (null, null);
            }
        }
    }
}
