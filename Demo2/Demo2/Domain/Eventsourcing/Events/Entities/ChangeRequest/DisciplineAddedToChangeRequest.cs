using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class DisciplineAddedToChangeRequest
{
    public class V1 : DomainEvent, IDomainEvent<ChangeRequest>
    {
        public V1(string username, Guid changeRequestId, Guid disciplineId) : base(username, changeRequestId)
        {
            DisciplineId = disciplineId;
        }

        public Guid DisciplineId { get; protected set; }

        public void ApplyTo(ChangeRequest entity)
        {
            entity.AddDiscipline(DisciplineId);
        }
    }
}