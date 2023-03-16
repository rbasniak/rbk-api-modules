using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestRequestedByUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string requestedBy) : base(username, aggregateId)
        {
            RequestedBy = requestedBy;
        }

        public string RequestedBy { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RequestedBy = RequestedBy;
        }
    }
}