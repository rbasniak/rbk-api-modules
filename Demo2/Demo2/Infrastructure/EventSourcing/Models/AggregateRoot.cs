using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Infrastructure;

public abstract class AggregateRoot<TSelf> : Entity, IAggregateRoot
  where TSelf : AggregateRoot<TSelf>
{
    private readonly List<IDomainEvent<TSelf>> _uncommittedEvents = new List<IDomainEvent<TSelf>>();

    protected AggregateRoot() { }

    protected AggregateRoot(IEnumerable<IDomainEvent<TSelf>> events)
    {
        if (events == null) return;

        foreach (var domainEvent in events)
        {
            Apply(domainEvent);

            Version = domainEvent.Version;
        }
    }

    public int Version { get; private set; }

    public IEnumerable<IDomainEvent<TSelf>> UncommittedEvents => _uncommittedEvents.ToList();

    public void Apply(IDomainEvent<TSelf> @event) => @event.ApplyTo((TSelf)this);

    public void Execute(ICommand<TSelf> command)
    {
        var events = command.ExecuteOn((TSelf)this);

        //if (command.ExpectedVersion != Version)
        //{
        //    throw new SynchronizationLockException();
        //}

        foreach (var @event in events)
        {
            Apply(@event);
            @event.Version = Version + 1;
            Version = Version + 1;
        }

        _uncommittedEvents.AddRange(events);
    }
}

public interface IDomainEvent
{
    int Version { get; set; }
    Guid EventId { get; }
    Guid AggregateId { get; }
    DateTime CreatedAt { get; }
    EventSummary Summary { get; }
}

public interface IDomainEvent<T>: IDomainEvent
{
    void ApplyTo(T entity);
}

public interface ICommand<T>
{
    IEnumerable<IDomainEvent<T>> ExecuteOn(T entity);
}

public interface IAggregateRoot : IEntity
{
    int Version { get; } 
}

//public abstract class AggregateRoot : Entity, IAggregateRoot
//{
//    private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

//    public int Version { get; }

//    protected AggregateRoot() { }

//    protected AggregateRoot(IEnumerable<IDomainEvent> events)
//    {
//        if (events == null) return;

//        foreach (var domainEvent in events)
//        {
//            Mutate(domainEvent);

//            Version = domainEvent.Version;
//        }
//    }

//    protected void AddDomainEvent(IDomainEvent @event) => _domainEvents.Add(@event);

//    protected void RemoveDomainEvent(IDomainEvent @event) => _domainEvents.Remove(@event);

//    internal void ClearDomainEvents() => _domainEvents.Clear();

//    internal IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

//    protected void Apply(IEnumerable<IDomainEvent> events)
//    {
//        foreach (var @event in events)
//        {
//            Apply(@event);
//        }
//    }

//    protected void Apply(IDomainEvent @event)
//    {
//        Mutate(@event);
//        AddDomainEvent(@event);
//    }

//    private void Mutate(IDomainEvent @event) => ((dynamic)this).On((dynamic)@event);
//}

//public interface IAggregateRoot : IEntity
//{
//    int Version { get; }
//    // IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
//}