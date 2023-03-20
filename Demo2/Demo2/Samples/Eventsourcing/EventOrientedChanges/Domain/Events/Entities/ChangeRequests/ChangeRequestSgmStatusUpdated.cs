using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.ChangeRequests;

public class ChangeRequestSgmStatusUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string sgmStatus) : base(username, aggregateId)
        {
            StatusSgm = sgmStatus;
        }

        public string StatusSgm { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.StatusSgm = StatusSgm;
        }
    }
}