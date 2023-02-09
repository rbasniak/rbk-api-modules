﻿using Demo2.Domain.Events.Infrastructure;
using Demo2.Domain.Models;
using Demo2.Infrastructure.EventSourcing.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.Repositories;

public interface IChangeRequestRepository
{
    Task<Guid> Create(string requestedBy, string createdBy, string description, string title);
    Task<ChangeRequest> FindAsync(Guid id);
    Task SaveAsync(ChangeRequest changeRequest);
}

public class ChangeRequestRepository: IChangeRequestRepository
{
    private readonly IEventStore _eventStore;
    public ChangeRequestRepository(IEventStore eventStore)
    {
        _eventStore = eventStore;
    }

    public async Task<Guid> Create(string requestedBy, string createdBy, string description, string title)
    {
        var request = ChangeRequest.CreateByGeneralUser(requestedBy, createdBy, description, title);

        await _eventStore.SaveAsync(request.Id, 0, request.DomainEvents);

        return request.Id;
    }

    public async Task<ChangeRequest> FindAsync(Guid id)
    {
        var events = await _eventStore.LoadAsync(id);

        return events.Count > 0 ? new ChangeRequest(events) : null;
    }

    public async Task SaveAsync(ChangeRequest changeRequest)
    {
        await _eventStore.SaveAsync(changeRequest.Id, changeRequest.Version, changeRequest.DomainEvents);
    }

    //public async Task<ChangeRequestId> SaveAsync(ChangeRequest person)
    //{
    //    await _eventStore.SaveAsync(person.Id, person.Version, person.DomainEvents);
    //    return person.Id;
    //}
}
