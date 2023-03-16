using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestDescriptionUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string description) : base(username, aggregateId)
        {
            Description = description;
        }

        public string Description { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Description = Description;
        }
    }
}