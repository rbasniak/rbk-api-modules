using Demo2.Domain.Events;
using Demo2.Domain.Events.Infrastructure;
using Demo2.EventSourcing;
using Demo2.Relational;
using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.EventSourcing;

public class ChangeRequest : AggregateRoot<ChangeRequest>
{
    HashSet<Fic> _fics = new();
    HashSet<Guid> _disciplines = new();
    HashSet<Document> _documents = new();
    HashSet<Attachment> _attachments = new();
    HashSet<EvidenceAttachment> _evidenceAttachments = new();

    public ChangeRequest()
    {
        Prioritization = new GutMatrix(0, 0, 0);
        DesiredDate = DateTime.Now.AddDays(180);
        CreationDate = DateTime.Now;
    }

    public ChangeRequest(IEnumerable<IDomainEvent<ChangeRequest>> events) : base(events)
    {
        Prioritization = new GutMatrix(0, 0, 0);
        DesiredDate = DateTime.Now.AddDays(180);
        CreationDate = DateTime.Now;
    }

    public override Guid Id { get; internal set; }

    public virtual Guid PlatformId { get; internal set; }

    public virtual Guid TypeId { get; internal set; }

    public virtual Guid SourceId { get; internal set; }

    public virtual string SourceNumber { get; internal set; }

    public virtual string RequestedBy { get; internal set; }

    public virtual IEnumerable<Guid> Disciplines => _disciplines?.ToList();

    public virtual string Description { get; internal set; }

    public virtual Guid PriorityId { get; internal set; }

    public virtual string Justification { get; internal set; }

    public virtual Guid StateId { get; internal set; }

    public virtual string StatusSgm { get; internal set; }

    public virtual string Comments { get; internal set; }

    public virtual double Complexity { get; internal set; }

    public virtual GutMatrix Prioritization { get; internal set; }

    public virtual string Resource { get; internal set; }
    public virtual string CreatedBy { get; internal set; }
    public virtual string CheckedBy { get; internal set; }
    public virtual string CurrentOwner { get; internal set; }
    public virtual DateTime CreationDate { get; internal set; }

    public virtual DateTime? DesiredDate { get; internal set; }

    public virtual long InternalNumber { get; internal set; }

    public virtual string FormattedInternalNumber => $"AB-{InternalNumber.ToString("00000")}";

    public virtual IEnumerable<Fic> Fics => _fics?.ToList();

    public virtual IEnumerable<Document> Documents => _documents?.ToList();

    public virtual IEnumerable<Attachment> Attachments => _attachments?.ToList();

    public virtual IEnumerable<EvidenceAttachment> EvidenceAttachments => _evidenceAttachments?.ToList();

    internal void RemoveDiscipline(Guid disciplineId)
    {
        _disciplines.Remove(disciplineId);
    }

    internal void AddDiscipline(Guid disciplineId)
    {
        _disciplines.Add(disciplineId);
    }

    internal void AddFic(Guid categoryId, string name)
    {
        _fics.Add(new Fic(Guid.NewGuid(), name, categoryId));
    }

    internal void RemoveFic(Guid ficId)
    {
        _fics.Remove(_fics.First(x => x.Id == ficId));
    }

    internal void UpdateFicName(Guid ficId, string name)
    {
        var fic = _fics.First(x => x.Id == ficId);

        fic.Name = name;
    }

    internal void UpdateFicCategory(Guid ficId, Guid categoryId)
    {
        var fic = _fics.First(x => x.Id == ficId);

        fic.CategoryId = categoryId;
    }

    internal void AddDocument(Guid categoryId, string name)
    {
        _documents.Add(new Document(Guid.NewGuid(), name, categoryId));
    }

    internal void RemoveDocument(Guid documentId)
    {
        _documents.Remove(_documents.First(x => x.Id == documentId));
    }

    internal void UpdateDocumentName(Guid documentId, string name)
    {
        var document = _documents.First(x => x.Id == documentId);

        document.Name = name;
    }

    internal void UpdateDocumentCategory(Guid documentId, Guid categoryId)
    {
        var document = _documents.First(x => x.Id == documentId);

        document.CategoryId = categoryId;
    }

    internal void AddAttachment(string name, Guid typeId, int size, string path, string filename)
    {
        _attachments.Add(new Attachment(name, typeId, size, path, filename));
    }

    internal void RemoveAttachment(Guid attachmentId)
    {
        _attachments.Remove(_attachments.First(x => x.Id == attachmentId));
    }

    internal void AddEvidenceAttachment(string name, Guid typeId, int size, string path, string filename, string comment)
    {
        _evidenceAttachments.Add(new EvidenceAttachment(name, typeId, size, path, filename, comment));
    }

    internal void RemoveEvidenceAttachment(Guid attachmentId)
    {
        _evidenceAttachments.Remove(_evidenceAttachments.First(x => x.Id == attachmentId));
    }
}
 