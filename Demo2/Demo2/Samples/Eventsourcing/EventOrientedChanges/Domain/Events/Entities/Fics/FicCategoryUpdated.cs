using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Fics;

public class FicCategoryUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid ficId, Guid categoryId) : base(username, aggregateId)
        {
            CategoryId = categoryId;
            FicId = ficId;
        }

        public Guid FicId { get; set; }
        public Guid CategoryId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateFicCategory(FicId, CategoryId);
        }
    }
}