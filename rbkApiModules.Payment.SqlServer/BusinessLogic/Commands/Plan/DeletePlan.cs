using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class DeletePlan
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.Id)
                    .MustExistInDatabase<Command, Plan>(_context)
                    .WithName("Id");
            } 
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor) : base(context, httpContextAccessor)
            {
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var plan = await _context.Set<Plan>().FindAsync(request.Id);

                _context.Remove(plan);

                await _context.SaveChangesAsync();

                return (plan.Id, null);
            }
        }
    }
}
