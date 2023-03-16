using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class FicRemovedFromChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid ficId) : base(username, aggregateId)
        {
            FicId = ficId;
        }

        public Guid FicId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RemoveFic(FicId);
        }
    }
}