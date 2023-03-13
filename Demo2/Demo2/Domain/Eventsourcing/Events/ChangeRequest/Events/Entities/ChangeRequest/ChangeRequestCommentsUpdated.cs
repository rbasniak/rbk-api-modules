using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestCommentsUpdated
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId, double complexity) : base(username, changeRequestId)
        {
            Complexity = complexity;
        }

        public double Complexity { get; set; }
    }
}