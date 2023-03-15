using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class FicCategoryUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid ficId, Guid categoryId) : base(username, changeRequestId)
        {
            CategoryId = categoryId;
            FicId = ficId;
        }

        public Guid FicId { get; protected set; }
        public Guid CategoryId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateFicCategory(FicId, CategoryId);
        }
    }
}