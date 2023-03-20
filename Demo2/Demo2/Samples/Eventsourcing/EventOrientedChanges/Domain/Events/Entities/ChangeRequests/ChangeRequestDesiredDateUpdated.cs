using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestDesiredDateUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, DateTime? desiredDate) : base(username, aggregateId)
        {
            DesiredDate = desiredDate;
        }

        public DateTime? DesiredDate { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.DesiredDate = DesiredDate;
        }
    }
}