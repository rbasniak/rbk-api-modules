using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public abstract class AggregateRoot : Entity, IAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

    public int Version { get; }

    protected AggregateRoot() { }

    protected AggregateRoot(IEnumerable<IDomainEvent> events)
    {
        if (events == null) return;

        foreach (var domainEvent in events)
        {
            Mutate(domainEvent);
            Version++;
        }
    }

    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

    protected void RemoveDomainEvent(IDomainEvent @event) => _domainEvents.Remove(@event);

    protected void ClearDomainEvents() => _domainEvents.Clear();

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void Apply(IEnumerable<IDomainEvent> events)
    {
        foreach (var @event in events)
        {
            Apply(@event);
        }
    }

    protected void Apply(IDomainEvent @event)
    {
        Mutate(@event);
        AddDomainEvent(@event);
    }

    private void Mutate(IDomainEvent @event) => ((dynamic)this).On((dynamic)@event);
}

public interface IAggregateRoot : IEntity
{
    int Version { get; }
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
}