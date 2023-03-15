using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestCurrentOwnerUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string currentOwner) : base(username, changeRequestId)
        {
            CurrentOwner = currentOwner;
        }

        public string CurrentOwner { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.CurrentOwner = CurrentOwner;
        }
    }
}