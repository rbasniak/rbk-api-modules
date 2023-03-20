using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestSourceUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid sourceId) : base(username, aggregateId)
        {
            SourceId = sourceId;
        }

        public Guid SourceId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.SourceId = SourceId;
        }
    }
}