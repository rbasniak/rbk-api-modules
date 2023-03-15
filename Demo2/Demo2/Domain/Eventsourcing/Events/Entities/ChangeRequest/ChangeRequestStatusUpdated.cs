using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestStatusUpdated
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId, object status) : base(username, changeRequestId)
        {
            Status = status;
        }

        public object Status { get; set; }
    }
}