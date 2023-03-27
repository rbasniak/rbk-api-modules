﻿using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Documents;

public class DocumentCategoryUpdatedOnChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid documentId, Guid categoryId) : base(username, aggregateId)
        {
            CategoryId = categoryId;
            DocumentId = documentId;
        }

        public Guid DocumentId { get; set; }
        public Guid CategoryId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.UpdateDocumentCategory(DocumentId, CategoryId);
        }
    }
}