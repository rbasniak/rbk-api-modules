using Demo2.Domain.Events;
using Demo2.Domain.Events.Domain;
using Demo2.Domain.Events.Infrastructure;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Commons.Relational.CQRS;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Demo2.Infrastructure.EventSourcing.Database.Repositories;

public interface IEventStore
{
    Task SaveAsync(IEnumerable<IDomainEvent> events);
    Task SaveAsync(IDomainEvent @event);

    Task<IEnumerable<IDomainEvent>> LoadAsync(Guid aggregateRootId);
    Task<IEnumerable<IDomainEvent>> LoadAllAsync(Type type);
}

public class RelationalEventStore : IEventStore
{
    private readonly DbContext _context;

    public RelationalEventStore(IEnumerable<DbContext> contexts)
    {
        _context = contexts.GetEventStoreContext();
    }

    public async Task<IEnumerable<IDomainEvent>> LoadAllAsync(Type aggregateType)
    {
        var events = await _context.Set<DomainEventDataObject>()
            // TODO: salvar o tipo do agregado na store 
            // .Where(x => x.Type == type.FullName)
            .ToListAsync();

        var results = new List<IDomainEvent>();

        foreach (var item in events)
        {
            var type = Type.GetType(item.Type);

            if (type is null)
            {
                throw new InvalidCastException($"Cannot find the type '{item.Type}' for an event");
            }

            var @event = JsonSerializer.Deserialize(item.Data, type);

            if (@event is null)
            {
                throw new InvalidCastException($"Cannot deserialize the event '{item.Id}' to the type '{item.Type}'");
            }

            results.Add((IDomainEvent)@event);
        }

        return results.AsReadOnly();
    }

    public async Task<IEnumerable<IDomainEvent>> LoadAsync(Guid aggregateRootId)
    {
        var results = new List<IDomainEvent>();

        var data = _context.Set<DomainEventDataObject>()
            .Where(x => x.AggregateId == aggregateRootId)
            .OrderBy(x => x.Version)
            .ToList();

        foreach (var item in data)
        {
            var type = Type.GetType(item.Type);

            if (type is null)
            {
                throw new InvalidCastException($"Cannot find the type '{item.Type}' for an event");
            }

            var @event = JsonSerializer.Deserialize(item.Data, type);

            if (@event is null)
            {
                throw new InvalidCastException($"Cannot deserialize the event '{item.Id}' to the type '{item.Type}'");
            }

            results.Add((IDomainEvent)@event);
        }

        return results.AsReadOnly();
    }

    public async Task SaveAsync(IEnumerable<IDomainEvent> events)
    {
        await _context.AddRangeAsync(events.Select(x => new DomainEventDataObject(x)));
        await _context.SaveChangesAsync();
    }

    public async Task SaveAsync(IDomainEvent @event)
    {
        await SaveAsync(new List<IDomainEvent> { @event });
    }
}
