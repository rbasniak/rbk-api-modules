using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestPlatformUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid platformId) : base(username, aggregateId)
        {
            PlatformId = platformId;
        }

        public Guid PlatformId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.PlatformId = PlatformId;
        }
    }
}