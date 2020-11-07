using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models;
using System;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class CreateUser
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Name { get; set; }
            public Guid UserId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DatabaseContext _context;

            public Validator(DatabaseContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.UserId)
                    .MustExistInDatabase<Command, User>(context);

                RuleFor(x => x.Username)
                    .MustAsync(MustNotExist).WithMessage("Usuário já existente");
            }

            private async Task<bool> MustNotExist(Command command, string username, CancellationToken arg2)
            {
                return !(await _context.Users.AnyAsync(x => EF.Functions.Like(x.Username, username)));
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
                var user = new User(request.Username, request.Password, true, new Client(request.Name, DateTime.Now));

                _context.Add(user);

                await _context.SaveChangesAsync();

                return (null, user);
            }
        }
    }
}
