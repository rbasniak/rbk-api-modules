using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Events.Entities.Attachments;

public class AttachmentRemovedFromChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, Guid attachmentId) : base(username, aggregateId)
        {
            AttachmentId = attachmentId;
        }

        public Guid AttachmentId { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RemoveAttachment(AttachmentId);
        }
    }
}