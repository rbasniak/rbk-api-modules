namespace Demo2.Domain.Events;

public abstract class DomainEvent : IDomainEvent
{
    protected DomainEvent()
    {
    }


    protected DomainEvent(Guid aggregateId)
    {
        EventId = Guid.NewGuid();
        AggregateId = aggregateId;
        CreatedAt = DateTime.UtcNow;
        Version = 1;
    }

    public Guid EventId { get; }

    public Guid AggregateId { get; }

    public int Version { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public abstract string Description { get; } 
}

public interface IDomainEvent
{
    Guid EventId { get; }
    Guid AggregateId { get; }
    int Version { get; }
    DateTime CreatedAt { get; }
}