using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class DisciplineRemovedFromChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid disciplineId) : base(username, aggregateId)
        {
            DisciplineId = disciplineId;
        }

        public Guid DisciplineId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RemoveDiscipline(DisciplineId);
        }
    }
}