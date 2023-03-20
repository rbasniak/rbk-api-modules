using Demo2.Samples.Eventsourcing.EventOrientedChanges.Domain.Models;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Repositories;
using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Models;
using MediatR;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Database.Repositories;

public interface IChangeRequestRepository
{
    Task<ChangeRequest[]> GetAllAsync();
    Task<ChangeRequest> FindAsync(Guid id);
    Task SaveAsync(ChangeRequest changeRequest);
}

public class ChangeRequestRepository : IChangeRequestRepository
{
    private readonly IEventStore _eventStore;
    private readonly IMediator _mediator;

    public ChangeRequestRepository(IEventStore eventStore, IMediator mediator)
    {
        _eventStore = eventStore;
        _mediator = mediator;
    }

    public async Task<ChangeRequest> FindAsync(Guid id)
    {
        var events = await _eventStore.LoadAsync(id);

        return events.Count() > 0 ? new ChangeRequest(events.Select(x => (IDomainEvent<ChangeRequest>)x)) : null;
    }

    public async Task<ChangeRequest[]> GetAllAsync()
    {
        var events = await _eventStore.LoadAllAsync(typeof(ChangeRequest));

        var groups = events.GroupBy(x => x.AggregateId).ToList();

        var results = new List<ChangeRequest>();

        foreach (var group in groups)
        {
            results.Add(new ChangeRequest(group.Select(x => (IDomainEvent<ChangeRequest>)x)));
        }

        return results.ToArray();
    }

    public async Task SaveAsync(ChangeRequest changeRequest)
    {
        //await _eventStore.SaveAsync(changeRequest.Id, changeRequest.Version, changeRequest.DomainEvents);

        //foreach (var @event in changeRequest.DomainEvents)
        //{
        //    await _mediator.Send(@event);
        //}
    }

    //public async Task<ChangeRequestId> SaveAsync(ChangeRequest person)
    //{
    //    await _eventStore.SaveAsync(person.Id, person.Version, person.DomainEvents);
    //    return person.Id;
    //}
}
