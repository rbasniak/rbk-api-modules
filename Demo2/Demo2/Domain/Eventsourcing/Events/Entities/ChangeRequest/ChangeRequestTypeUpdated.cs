using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestTypeUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid typeId) : base(username, changeRequestId)
        {
            TypeId = typeId;
        }

        public Guid TypeId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            TypeId = entity.TypeId;
        }
    }
}