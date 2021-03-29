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
    public class CreateTrialKey
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid ClientId { get; set; }
            public Guid PlanId { get; set; } 
            public int Days { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.PlanId)
                    .MustExistInDatabase<Command, Plan>(context).WithMessage("Plano não encontrado");

                RuleFor(x => x.ClientId)
                    .MustExistInDatabase<Command, Plan>(context)
                    .WithMessage("Cliente não encontrado");

                RuleFor(x => x.Days)
                    .GreaterThan(0).WithMessage("O número de dias deve ser maior que zero");
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly ITrialKeyActions _trialKeyActions;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, ITrialKeyActions trialKeyActions)
                : base(context, httpContextAccessor)
            {
                _trialKeyActions = trialKeyActions;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var client = await _context.Set<BaseClient>().FindAsync(request.ClientId);
                var plan = await _context.Set<Plan>().FindAsync(request.PlanId);

                var trialKey = new TrialKey(plan, request.Days);

                await _context.AddAsync(trialKey);

                client.SetTrialKey(trialKey);

                await _context.SaveChangesAsync();

                _trialKeyActions.OnCreateSuccess(request.ClientId, trialKey.Id, request.Days.ToString(), plan.Name);

                return (trialKey.Id, trialKey);
            }
        }
    }
}
