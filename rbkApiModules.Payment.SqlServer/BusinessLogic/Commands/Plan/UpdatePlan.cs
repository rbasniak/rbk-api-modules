using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class UpdatePlan
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public bool IsActive { get; set; }
            public int Duration { get; set; }
            public double Price { get; set; }
            public bool IsDefault { get; set; }
            public string PaypalId { get; set; }
            public string PaypalSandboxId { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(a => a.Id)
                    .MustExistInDatabase<Command, Plan>(_context)
                    .WithName("Id");

                RuleFor(a => a.Name)
                    .IsRequired()
                    .WithName("Nome");

                RuleFor(a => a.IsDefault)
                   .MustAsync(CanSetDefault)
                   .WithMessage("Já existe um plano padrão definido.");
            }

            public async Task<bool> CanSetDefault(Command command, bool IsDefaultPlan, CancellationToken cancelation)
            {
                if (IsDefaultPlan)
                {
                    return !await _context.Set<Plan>().AnyAsync(x => x.IsDefault, cancellationToken: cancelation);
                }
                else
                {
                    return true;
                }
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

                plan.Update(
                    request.Name,
                    request.Duration,
                    request.Price,
                    request.IsActive,
                    request.IsDefault,
                    request.PaypalId,
                    request.PaypalSandboxId);

                await _context.SaveChangesAsync();

                return (plan.Id, plan);
            }
        }
    }
}
