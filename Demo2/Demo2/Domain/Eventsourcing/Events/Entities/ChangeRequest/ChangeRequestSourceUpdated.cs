using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestSourceUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid sourceId) : base(username, changeRequestId)
        {
            SourceId = sourceId;
        }

        public Guid SourceId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.SourceId = SourceId;
        }
    }
}