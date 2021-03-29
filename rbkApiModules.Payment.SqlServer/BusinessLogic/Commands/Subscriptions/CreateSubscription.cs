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
    public class CreateSubscription
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid ClientId { get; set; }
            public Guid PlanId { get; set; }
            public string BillingToken { get; set; }
            public string FacilitatorAccessToken { get; set; } 
            public string OrderID { get; set; }
            public string SubscriptionID { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.ClientId)
                    .MustExistInDatabase<Command, Plan>(context)
                    .WithMessage("Cliente não encontrado");

                RuleFor(x => x.PlanId)
                    .MustExistInDatabase<Command, Plan>(context).WithMessage("INVALID_PlANID");
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly ISubscriptionActions _subscriptionActions;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, ISubscriptionActions subscriptionActions)
                : base(context, httpContextAccessor)
            {
                _subscriptionActions = subscriptionActions;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var client = await _context.Set<BaseClient>().FindAsync(request.ClientId);
                var plan = await _context.Set<Plan>().FindAsync(request.PlanId);

                var subscription = new Subscription(client, plan, request.BillingToken,
                                                    request.FacilitatorAccessToken, request.OrderID, request.SubscriptionID);

                client.ChangePlan(plan);
                client.SetSubscriptionInCancelation(false);
                client.SetSubscriptionHasExpired(false);
                
                client.RemoveTrial();

                await _context.AddAsync(subscription);

                await _context.SaveChangesAsync();

                _subscriptionActions.OnCreateSuccess(request.ClientId, plan.Name);

                return (subscription.Id, subscription);
            }
        }
    }
}
