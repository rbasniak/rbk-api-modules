using Demo2.Domain.Events;
using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.EventSourcing;

public class ChangeRequest :  AggregateRoot
{
     HashSet<Fic> _fics;
     HashSet<Document> _documents;
     HashSet<Attachment> _attachments;
     HashSet<EvidenceAttachment> _evidenceAttachments;

    protected ChangeRequest()
    {
    }

     ChangeRequest(Platform platform, ChangeRequestType type, ChangeRequestPriority priority,
        ChangeRequestSource source, ChangeRequestState state, string description, string justification,
        string createdBy, string requestedBy, string currentOwner, string sourceNumber, DateTime? desiredDate)
    {
        _fics = new HashSet<Fic>();
        _documents = new HashSet<Document>();
        _attachments = new HashSet<Attachment>();
        _evidenceAttachments = new HashSet<EvidenceAttachment>();

        PlatformId = platform.Id;
        Type = type;
        Priority = priority;
        Source = source;
        Description = description;
        Justification = justification;
        CreationDate = DateTime.Now;
        DesiredDate = desiredDate ?? DateTime.Now.AddDays(180);
        State = state;
        SourceNumber = sourceNumber;
        RequestedBy = requestedBy;
        CreatedBy = createdBy;
        CurrentOwner = currentOwner;

        Prioritization = new GutMatrix(0, 0, 0);
    }
    public override Guid Id { get; protected set; }


    public virtual Guid PlatformId { get;  set; } 

    public virtual ChangeRequestType Type { get;  set; }

    public virtual ChangeRequestSource Source { get;  set; }

    public virtual string SourceNumber { get;  set; }

    public virtual string RequestedBy { get;  set; }

    public virtual List<Discipline> Disciplines { get;  set; }

    public virtual string Description { get;  set; }

    public virtual ChangeRequestPriority Priority { get;  set; }

    public virtual string Justification { get;  set; }

    public virtual ChangeRequestState State { get;  set; }

    public virtual string StatusSgm { get;  set; }

    public virtual string Comments { get;  set; }

    public virtual double Complexity { get;  set; }

    public virtual GutMatrix Prioritization { get;  set; }

    public virtual string Resource { get;  set; }
    public virtual string CreatedBy { get;  set; }
    public virtual string CheckedBy { get;  set; }
    public virtual string CurrentOwner { get;  set; }
    public virtual DateTime CreationDate { get;  set; }

    public virtual DateTime? DesiredDate { get;  set; }

    public virtual long InternalNumber { get;  set; }

    public virtual string FormattedInternalNumber => $"AB-{InternalNumber.ToString("00000")}";

    public virtual List<Fic> Fics { get;  set; }

    public virtual List<Document> Documents { get;  set; }

    public virtual List<Attachment> Attachments { get;  set; }

    public virtual List<EvidenceAttachment> EvidenceAttachments { get;  set; }

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


public enum ChangeRequestState
{

}

public enum ChangeRequestSource
{

}

public enum Discipline
{

}