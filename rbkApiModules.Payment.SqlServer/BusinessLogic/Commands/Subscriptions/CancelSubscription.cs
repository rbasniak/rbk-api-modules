using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Infrastructure.MediatR.Core;
using rbkApiModules.Infrastructure.MediatR.SqlServer;
using rbkApiModules.Paypal.SqlServer;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class CancelSubscription
    {
        public class Command : IRequest<CommandResponse>
        {
            public Guid ClientId { get; set; }
            public string Reason { get; set; }
        }

        public class Validator : AbstractValidator<Command>
        {
            private readonly DbContext _context;

            public Validator(DbContext context)
            {
                _context = context;

                CascadeMode = CascadeMode.Stop;

                RuleFor(x => x.ClientId)
                    .MustExistInDatabase<Command, BaseClient>(context)
                    .WithName("Cliente");

                RuleFor(x => x.Reason)
                    .IsRequired()
                    .WithName("Razão");
            }
        }

        public class Handler : BaseCommandHandler<Command, DbContext>
        {
            private readonly IPaypalService _paypalService;
            private readonly ISubscriptionActions _subscriptionActions;

            public Handler(DbContext context, IHttpContextAccessor httpContextAccessor, ISubscriptionActions subscriptionActions,
                IPaypalService paypalService)
                : base(context, httpContextAccessor)
            {
                _paypalService = paypalService;
                _subscriptionActions = subscriptionActions;
            }

            protected override async Task<(Guid? entityId, object result)> ExecuteAsync(Command request)
            {
                var client = await _context.Set<BaseClient>().FindAsync(request.ClientId);

                var subscription = _context.Set<Subscription>()
                    .Include(x => x.Plan)
                    .Where(x => x.ClientId == request.ClientId)
                    .OrderBy(x => x.SubscriptionDate)
                    .LastOrDefault();

                var token = await _paypalService.LoginAsync();

                var returnMessage = await _paypalService
                    .CancelSubscriptionAsync(token, subscription.SubscriptionID, request.Reason);

                if (returnMessage != null)
                {
                    throw new Exception(returnMessage.Details.First().Description);
                }

                client.SetSubscriptionInCancelation(true);

                await _context.SaveChangesAsync();

                _subscriptionActions.OnCancelationSuccess(request.ClientId, subscription.Plan.Name);

                return (null, null);
            }
        }
    }
}
