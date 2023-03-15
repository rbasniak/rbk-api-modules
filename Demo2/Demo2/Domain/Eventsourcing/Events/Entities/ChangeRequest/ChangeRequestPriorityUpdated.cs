using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestPriorityUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid priorityId) : base(username, changeRequestId)
        {
            PriorityId = priorityId;
        }

        public Guid PriorityId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.PriorityId = PriorityId;
        }
    }
}