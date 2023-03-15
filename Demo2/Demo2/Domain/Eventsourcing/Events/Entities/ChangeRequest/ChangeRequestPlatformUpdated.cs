using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestPlatformUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid platformId) : base(username, changeRequestId)
        {
            PlatformId = platformId;
        }

        public Guid PlatformId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.PlatformId = PlatformId;
        }
    }
}