using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class DocumentCategoryUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid documentId, Guid categoryId) : base(username, changeRequestId)
        {
            CategoryId = categoryId;
            DocumentId = documentId;
        }

        public Guid DocumentId { get; protected set; }
        public Guid CategoryId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateDocumentCategory(DocumentId, CategoryId);
        }
    }
}