using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestTypeUpdated
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId, int gravity, int urgency, int tendency) : base(username, changeRequestId)
        {
            Gravity = gravity;
            Tendency = tendency;
            Urgency = urgency;
        }

        public int Gravity { get; set; }
        public int Urgency { get; set; }
        public int Tendency { get; set; }
    }
}