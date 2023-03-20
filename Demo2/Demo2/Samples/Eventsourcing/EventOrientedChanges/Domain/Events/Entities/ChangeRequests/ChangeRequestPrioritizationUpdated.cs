using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestPrioritizationUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, int gravity, int urgency, int tendency) : base(username, aggregateId)
        {
            Gravity = gravity;
            Tendency = tendency;
            Urgency = urgency;
        }

        public int Gravity { get; set; }
        public int Urgency { get; set; }
        public int Tendency { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Prioritization = new GutMatrix(Gravity, Urgency, Tendency);
        }
    }
}