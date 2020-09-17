using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Tester.Database;
using rbkApiModules.Tester.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Tester.BusinessLogic 
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

                _context.SaveChanges();

                return (null, user);
            }
        }
    }
}
