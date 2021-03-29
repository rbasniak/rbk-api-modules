using rbkApiModules.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace rbkApiModules.Payment.SqlServer
{
    public class Subscription : BaseEntity
    {
        private readonly HashSet<Payment> _payments;

        protected Subscription() { }

        public Subscription(
            BaseClient client,
            Plan plan,
            string billingToken,
            string facilitatorAccessToken,
            string orderID,
            string subscriptionID)
        {
            _payments = new HashSet<Payment>();

            Client = client;
            Plan = plan;
            SubscriptionDate = DateTime.UtcNow;
            Amount = plan.Price;
            BillingToken = billingToken;
            FacilitatorAccessToken = facilitatorAccessToken;
            OrderID = orderID;
            SubscriptionID = subscriptionID;
        }

        public Guid ClientId { get; set; }
        public BaseClient Client { get; set; }
        public Guid PlanId { get; private set; }
        public Plan Plan { get; private set; }
        public DateTime SubscriptionDate { get; private set; }
        public string BillingToken { get; private set; }
        public string FacilitatorAccessToken { get; private set; }
        public string OrderID { get; private set; }
        public string SubscriptionID { get; private set; }
        public double Amount { get; private set; }

        public IEnumerable<Payment> Payments => _payments?.ToList();
    }
}
