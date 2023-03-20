using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestSourceNumberUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string sourceNumber) : base(username, aggregateId)
        {
            SourceNumber = sourceNumber;
        }

        public string SourceNumber { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.SourceNumber = SourceNumber;
        }
    }
}