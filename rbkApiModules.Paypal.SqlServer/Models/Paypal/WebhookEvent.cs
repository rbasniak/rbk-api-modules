using rbkApiModules.Infrastructure.Models;
using System;

namespace rbkApiModules.Paypal.SqlServer
{
    public class WebhookEvent : BaseEntity
    {
        protected WebhookEvent()
        {
        }

        public WebhookEvent(
            string creationTime,
            string eventType,
            string eventContent,
            string resourceId,
            string planId,
            string subscriberEmail,
            string validationStatus)
        {
            CreationTime = creationTime;
            ReceivedTime = DateTime.UtcNow;
            EventType = eventType;
            EventContent = eventContent;
            ResourceId = resourceId;
            PlanId = planId;
            SubscriberEmail = subscriberEmail;
            ValidationStatus = validationStatus;
        }

        public string CreationTime { get; private set; }
        public DateTime ReceivedTime { get; private set; }
        public string EventType { get; private set; }
        public string EventContent { get; private set; }
        public string ResourceId { get; private set; }
        public string PlanId { get; private set; }
        public string SubscriberEmail { get; private set; }
        public string ValidationStatus { get; private set; }
    }
}
