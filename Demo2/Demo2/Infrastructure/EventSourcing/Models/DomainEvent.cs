using MediatR;
using Newtonsoft.Json;

namespace Demo2.Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
    }


    protected DomainEvent(string username, Guid aggregateId)
    {
        Version = 1;
        CreatedBy = username;
        EventId = Guid.NewGuid();
        AggregateId = aggregateId;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid EventId { get; }

    public Guid AggregateId { get; }

    public int Version { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public string CreatedBy { get; private set; }

    [JsonIgnore]
    public virtual EventSummary Summary { get; } 
}

public interface IDomainEvent: INotification
{
    Guid EventId { get; }
    Guid AggregateId { get; }
    int Version { get; }
    DateTime CreatedAt { get; }
    EventSummary Summary { get; }
}

public class EventSummary
{
    public string Description { get; set; }
    public Dictionary<string, string> ChangedProperties { get; set; }
}