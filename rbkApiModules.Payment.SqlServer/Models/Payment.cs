using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Payment.SqlServer
{
    public class Payment : BaseEntity
    {
        private Payment() { }

        public Payment(
            Subscription subscription,
            string paymentID)
        {
            Subscription = subscription;
            PaymentDate = DateTime.UtcNow;
            PaymentID = paymentID;
        }

        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; }
        public DateTime PaymentDate { get; private set; }
        public string PaymentID { get; private set; }
    }
}
