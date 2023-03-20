using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Documents;

public class DocumentNameUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid documentId, string name) : base(username, aggregateId)
        {
            Name = name;
            DocumentId = documentId;
        }

        public Guid DocumentId { get; set; }
        public string Name { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateDocumentName(DocumentId, Name);
        }
    }
}