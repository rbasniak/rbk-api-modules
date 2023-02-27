using Demo2.Domain.Events;
using Demo2.Domain.Events.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using System;

namespace Demo2.Domain.Models;

public class ChangeRequest: AggregateRoot
{
    private readonly HashSet<Fic> _fics;
    private readonly HashSet<Document> _documents;

    private ChangeRequest()
    {
        _fics = new HashSet<Fic>();
        _documents = new HashSet<Document>();
    }

    public ChangeRequest(IEnumerable<IDomainEvent> events) : base(events)
    {
        _fics = new HashSet<Fic>();
        _documents = new HashSet<Document>();
    }

    public override Guid Id { get; protected set; }
    public string RequestedBy { get; protected set; }
    public string CreatedBy { get; protected set; }
    public string Description { get; protected set; }
    public string Title { get; protected set; }
    public IEnumerable<Fic> Fics => _fics.ToList();
    public IEnumerable<Document> Documents => _documents.ToList();

    public static ChangeRequest CreateByGeneralUser(string requestedBy, string createdBy, string description, string title)
    {
        var @event = new ChangeRequestCreatedByGeneralUser.V1(Guid.NewGuid(), requestedBy, createdBy, description, title);
        var changeRequest = new ChangeRequest();
        changeRequest.Apply(@event);

        return changeRequest;
    }

    public void On(ChangeRequestCreatedByGeneralUser.V1 @event)
    {
        Id = @event.AggregateId;
        RequestedBy = @event.RequestedBy;
        CreatedBy = @event.CreatedBy;
        Description = @event.Description2;
        Title = @event.Title;
    }

    public void On(FicAddedtoChangeRequest.V1 @event)
    {
        var fic = new Fic(@event.Number, @event.Name, @event.Source);
        _fics.Add(fic);
    }

    public void On(DocumentAddedToChangeRequest.V1 @event)
    {
        var document = new Document(@event.Number, @event.Name, @event.Source);
        _documents.Add(document);
    }

    public void On(FicRemovedFromChangeRequest.V1 @event)
    {
        _fics.Remove(_fics.First(x => x.Id == @event.EventId));
    }

    internal void AddFic(string name, string number, string source)
    {
        var @event = new FicAddedtoChangeRequest.V1(Id, name, number, source);
        Apply(@event);
    }

    internal void AddDocument(string name, string number, string source)
    {
        var @event = new DocumentAddedToChangeRequest.V1(Id, name, number, source);
        Apply(@event);
    }

    internal void RemoveFic(Guid ficId)
    {
        var @event = new FicRemovedFromChangeRequest.V1(Id, ficId);
        Apply(@event);
    }
} 