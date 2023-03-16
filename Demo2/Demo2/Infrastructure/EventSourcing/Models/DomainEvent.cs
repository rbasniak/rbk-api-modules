using MediatR;
using Newtonsoft.Json;

namespace Demo2.Domain.Events;

public abstract class DomainEvent  
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

    public Guid EventId { get; set; }

    public Guid AggregateId { get; set; }

    public int Version { get; set; }

    public DateTime CreatedAt { get; set; }

    public string CreatedBy { get; set; }

    [JsonIgnore]
    public virtual EventSummary Summary { get; } 
}

public class EventSummary
{
    public string Description { get; set; }
    public Dictionary<string, string> ChangedProperties { get; set; }
}