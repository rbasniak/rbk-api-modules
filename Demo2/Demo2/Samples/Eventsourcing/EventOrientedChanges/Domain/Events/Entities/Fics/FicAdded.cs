using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Fics;

public class FicAddedToChangeRequest
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
            entity.AddFic(CategoryId, Name);
        }
    }
}