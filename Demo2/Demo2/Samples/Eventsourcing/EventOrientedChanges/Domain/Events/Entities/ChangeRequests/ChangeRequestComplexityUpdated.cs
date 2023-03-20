using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestComplexityUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, double complexity) : base(username, aggregateId)
        {
            Complexity = complexity;
        }

        public double Complexity { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Complexity = Complexity;
        }
    }
}