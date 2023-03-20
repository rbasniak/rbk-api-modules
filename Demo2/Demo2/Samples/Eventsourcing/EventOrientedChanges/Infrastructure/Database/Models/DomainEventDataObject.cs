using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using System.Text.Json;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Models;

public class DomainEventDataObject
{
    protected DomainEventDataObject()
    {

    }

    public DomainEventDataObject(IDomainEvent @event)
    {
        Id = @event.EventId;
        AggregateId = @event.AggregateId;
        CreatedAt = @event.CreatedAt;
        Version = @event.Version;
        Type = @event.GetType().FullName;
        Data = JsonSerializer.Serialize((object)@event);
    }

    public Guid Id { get; protected set; }
    public Guid AggregateId { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public int Version { get; protected set; }
    public string Type { get; protected set; }
    public string Data { get; protected set; }
}
