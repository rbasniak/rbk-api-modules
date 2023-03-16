using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class DocumentAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, string name, Guid categoryId) : base(username, aggregateId)
        {
            Name = name;
            CategoryId = categoryId;
        }

        public Guid CategoryId { get; set; }
        public string Name { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddDocument(CategoryId, Name);
        }
    }
}