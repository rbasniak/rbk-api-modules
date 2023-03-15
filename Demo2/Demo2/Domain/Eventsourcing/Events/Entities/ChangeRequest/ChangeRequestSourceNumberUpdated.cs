using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestSourceNumberUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, string sourceNumber) : base(username, changeRequestId)
        {
            SourceNumber = sourceNumber;
        }

        public string SourceNumber { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.SourceNumber = SourceNumber;
        }
    }
}