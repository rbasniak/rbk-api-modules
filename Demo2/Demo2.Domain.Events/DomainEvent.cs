using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Demo2.Domain.Events;

public abstract class DomainEvent<T> : IDomainEvent
{
    protected DomainEvent()
    {
    }


    protected DomainEvent(Guid aggregateId, T payload)
    {
        Id = Guid.NewGuid();
        AggregateId = aggregateId;
        CreatedAt = DateTime.UtcNow;
        Version = 0;
        Sequence = 0;
        Payload = payload;
    }

    public Guid Id { get; }

    public Guid AggregateId { get; }

    public int Version { get; private set; }

    public int Sequence { get; private set; }

    public DateTime CreatedAt { get; private set; }
    
    public T Payload { get; private set; }

    public abstract string Description { get; } 
}

public interface IDomainEvent
{
    int Sequence { get; }

    int Version { get; }

    DateTime CreatedAt { get; }
}