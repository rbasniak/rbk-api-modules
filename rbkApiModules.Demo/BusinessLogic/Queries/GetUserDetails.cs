using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Infrastructure.MediatR.Core;
using System;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class GetUserDetails
    {
        public class Command : IRequest<QueryResponse>
        {
            // [ExistingEntity(typeof(User))]
            public Guid Id { get; set; }
        } 

        public class Validator: AbstractValidator<Command>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .Must(Test);
            }

            private bool Test(Guid arg)
            {
                throw new SafeException("Erro customizado.");
            }
        }

        public class Handler : BaseQueryHandler<Command, DatabaseContext>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor)
                : base(context, httpContextAccessor)
            {
            }

            protected async override Task<object> ExecuteAsync(Command request)
            {
                return await _context.Users.FirstOrDefaultAsync(x => x.Id == request.Id);
            }
        }
    }
}
