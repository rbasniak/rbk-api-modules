using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using System.Diagnostics;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestTypeUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid typeId) : base(username, aggregateId)
        {
            TypeId = typeId;
            if (TypeId == Guid.Empty) Debugger.Break();
        }

        public Guid TypeId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.TypeId = TypeId;
        }
    }
}