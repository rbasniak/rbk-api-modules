using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestStatusUpdated
{
    public class V1 : DomainEvent
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, object status) : base(username, aggregateId)
        {
            Status = status;
        }

        public object Status { get; set; }
    }
}