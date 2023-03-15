using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestRequestedByUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string requestedBy) : base(username, changeRequestId)
        {
            RequestedBy = requestedBy;
        }

        public string RequestedBy { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RequestedBy = RequestedBy;
        }
    }
}