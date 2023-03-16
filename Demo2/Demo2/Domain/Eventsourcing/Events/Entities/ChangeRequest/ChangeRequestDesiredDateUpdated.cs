using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestDesiredDateUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, DateTime? desiredDate) : base(username, aggregateId)
        {
            DesiredDate = desiredDate;
        }

        public DateTime? DesiredDate { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.DesiredDate = DesiredDate;
        }
    }
}