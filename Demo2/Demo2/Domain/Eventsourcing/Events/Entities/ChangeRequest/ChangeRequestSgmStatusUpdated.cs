using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestSgmStatusUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string sgmStatus) : base(username, changeRequestId)
        {
            StatusSgm = sgmStatus;
        }

        public string StatusSgm { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.StatusSgm = StatusSgm;
        }
    }
}