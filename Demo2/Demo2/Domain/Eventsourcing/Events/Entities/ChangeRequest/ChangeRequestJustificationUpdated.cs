using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestJustificationUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string justification) : base(username, changeRequestId)
        {
            Justification = justification;
        }

        public string Justification { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Justification = Justification;
        }
    }
}