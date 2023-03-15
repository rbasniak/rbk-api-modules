using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class FicAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string name, Guid categoryId) : base(username, changeRequestId)
        {
            Name = name;
            CategoryId = categoryId;
        }

        public Guid CategoryId { get; protected set; }
        public string Name { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddFic(CategoryId, Name);
        }
    }
}