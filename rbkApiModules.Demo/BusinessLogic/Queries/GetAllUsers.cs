using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Infrastructure.MediatR;
using rbkApiModules.Demo.Database;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.SqlServer;

namespace rbkApiModules.Demo.BusinessLogic
{
    public class GetAllDemoUsers
    {
        public class Command : IRequest<QueryResponse>
        {
             
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DatabaseContext _context;

            public Validator(DatabaseContext context)
            {
                
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
                return await _context.Users.ToListAsync();
            }
        }
    }
}
