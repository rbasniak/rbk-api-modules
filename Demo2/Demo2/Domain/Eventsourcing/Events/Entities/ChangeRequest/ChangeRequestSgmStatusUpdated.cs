using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

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