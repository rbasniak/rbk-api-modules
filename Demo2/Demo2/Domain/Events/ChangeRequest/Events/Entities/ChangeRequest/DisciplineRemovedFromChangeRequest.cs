using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class DisciplineRemovedFromChangeRequest
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId, Discipline discipline) : base(username, changeRequestId)
        {
            Discipline = discipline;
        }

        public Discipline Discipline { get; set; }
    }
}