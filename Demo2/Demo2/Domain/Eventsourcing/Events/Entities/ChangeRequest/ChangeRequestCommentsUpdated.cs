using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestCommentsUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string comments) : base(username, changeRequestId)
        {
            Comments = comments;
        }

        public string Comments { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Comments = Comments;
        }
    }
}