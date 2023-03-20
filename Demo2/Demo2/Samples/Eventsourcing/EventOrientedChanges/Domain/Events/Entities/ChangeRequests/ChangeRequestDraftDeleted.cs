using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestDraftDeleted
{
    public class V1 : DomainEvent
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId) : base(username, aggregateId)
        {
        }
    }
}