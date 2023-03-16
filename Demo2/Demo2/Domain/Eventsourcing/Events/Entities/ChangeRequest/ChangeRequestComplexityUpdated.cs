using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestComplexityUpdated
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1()
        {

        }
        public V1(string username, Guid aggregateId, double complexity) : base(username, aggregateId)
        {
            Complexity = complexity;
        }

        public double Complexity { get; set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.Complexity = Complexity;
        }
    }
}