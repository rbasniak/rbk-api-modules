using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class AttachmentRemovedFromChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid attachmentId) : base(username, changeRequestId)
        {
            AttachmentId = attachmentId;
        }

        public Guid AttachmentId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.RemoveAttachment(AttachmentId);
        }
    }
}