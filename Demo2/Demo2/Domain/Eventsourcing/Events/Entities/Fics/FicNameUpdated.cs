using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class FicNameUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid ficId, string name) : base(username, changeRequestId)
        {
            Name = name;
            FicId = ficId;
        }

        public Guid FicId { get; protected set; }
        public string Name { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateFicName(FicId, Name);
        }
    }
}