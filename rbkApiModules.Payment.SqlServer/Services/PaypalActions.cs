using Microsoft.EntityFrameworkCore;
using rbkApiModules.Paypal.SqlServer;
using System.Threading.Tasks;

namespace rbkApiModules.Payment.SqlServer
{
    public class PaypalActions : IPaypalActions
    {
        private readonly DbContext _context;

        public PaypalActions(DbContext context)
        {
            _context = context;
        }

        public async Task OnWebhookEventReceived(WebhookEventResponse response)
        {
            if (response.EventType == "PAYMENT.SALE.COMPLETED")
            {
                var subscription = await _context.Set<Subscription>()
                    .SingleAsync(x => x.SubscriptionID == response.Resource.BillingAgreementId);

                if (! await _context.Set<Payment>().AnyAsync(x => x.PaymentID == response.Resource.Id && x.SubscriptionId == subscription.Id))
                {
                    var payment = new Payment(subscription, response.Resource.Id);

                    await _context.AddAsync(payment);
                }
            }
        }
    }
}
