using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class CreateUser
    {
        public class Command : IRequest<CommandResponse>
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            public Validator()
            {
                CascadeMode = CascadeMode.Stop;
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
                var user = new User(request.Username, request.Password, true, new Client("", DateTime.Now));

                _context.Add(user);

                await _context.SaveChangesAsync();

                return (null, user);
            }
        }
    }
}
