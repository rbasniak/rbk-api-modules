using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestDesiredDateUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, DateTime? desiredDate) : base(username, changeRequestId)
        {
            DesiredDate = desiredDate;
        }

        public DateTime? DesiredDate { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.DesiredDate = DesiredDate;
        }
    }
}