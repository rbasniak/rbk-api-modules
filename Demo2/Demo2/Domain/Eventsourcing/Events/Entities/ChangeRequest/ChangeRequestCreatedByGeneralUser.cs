using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestCreatedByGeneralUser
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId) : base(username, changeRequestId)
        {
        }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Id = AggregateId;
        }
    }
}