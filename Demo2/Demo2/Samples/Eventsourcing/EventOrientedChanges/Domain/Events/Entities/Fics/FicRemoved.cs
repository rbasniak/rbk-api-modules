using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Fics;

public class FicRemovedFromChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid ficId) : base(username, aggregateId)
        {
            FicId = ficId;
        }

        public Guid FicId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RemoveFic(FicId);
        }
    }
}