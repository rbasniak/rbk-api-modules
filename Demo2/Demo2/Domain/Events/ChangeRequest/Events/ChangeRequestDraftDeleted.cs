using Demo2.EventSourcing;
using System.Text.Json.Serialization;

namespace Demo2.Domain.Events;

public class ChangeRequestDraftDeleted
{
    public class V1 : DomainEvent
    {
        public V1(string username, Guid changeRequestId) : base(username, changeRequestId)
        {
        }
    }
}