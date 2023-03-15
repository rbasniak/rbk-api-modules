﻿using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Repositories;

public interface IChangeRequestRepository
{
    Task<ChangeRequest> FindAsync(Guid id);
    Task SaveAsync(ChangeRequest changeRequest);
}

public class ChangeRequestRepository: IChangeRequestRepository
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