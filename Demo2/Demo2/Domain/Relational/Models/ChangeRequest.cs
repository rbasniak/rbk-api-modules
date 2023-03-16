using rbkApiModules.Commons.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Demo2.Relational;

public class ChangeRequest : BaseEntity
{
    private HashSet<ChangeRequestToDiscipline> _disciplines;
    private HashSet<Fic> _fics;
    private HashSet<Document> _documents;
    private HashSet<Attachment> _attachments;
    private HashSet<EvidenceAttachment> _evidenceAttachments;
    private HashSet<StateChangeEvent> _events;

    protected ChangeRequest()
    {
    }

    public ChangeRequest(Platform platform, ChangeRequestType type, ChangeRequestPriority priority,
        ChangeRequestSource source, State state, string description, string justification,
        string createdBy, string requestedBy, string currentOwner, string sourceNumber, DateTime? desiredDate, bool isInTesting = false)
    {
        _disciplines = new HashSet<ChangeRequestToDiscipline>();
        _fics = new HashSet<Fic>();
        _documents = new HashSet<Document>();
        _attachments = new HashSet<Attachment>();
        _evidenceAttachments = new HashSet<EvidenceAttachment>();
        _events = new HashSet<StateChangeEvent>();

        Platform = platform;
        Type = type;
        Priority = priority;
        Source = source;
        Description = description;
        Justification = justification;
        CreationDate = DateTime.UtcNow;
        DesiredDate = desiredDate ?? DateTime.UtcNow.AddDays(180);
        State = state;
        SourceNumber = sourceNumber;
        RequestedBy = requestedBy;
        CreatedBy = createdBy;
        CurrentOwner = currentOwner;

        Prioritization = new GutMatrix(0, 0, 0);
    }

    public virtual Guid PlatformId { get; private set; }
    public virtual Platform Platform { get; private set; }

    public virtual Guid TypeId { get; private set; }
    public virtual ChangeRequestType Type { get; private set; }

    public virtual Guid SourceId { get; private set; }
    public virtual ChangeRequestSource Source { get; private set; }

    public virtual string SourceNumber { get; private set; }

    public virtual string RequestedBy { get; private set; }

    public virtual IEnumerable<ChangeRequestToDiscipline> Disciplines => _disciplines?.ToList();

    public virtual string Description { get; private set; }

    public virtual Guid PriorityId { get; private set; }
    public virtual ChangeRequestPriority Priority { get; private set; }

    public virtual string Justification { get; private set; }

    public virtual Guid StateId { get; private set; }
    public virtual State State { get; private set; }

    public virtual string StatusSgm { get; private set; }

    public virtual string Comments { get; private set; }

    public virtual double Complexity { get; private set; }

    public virtual GutMatrix Prioritization { get; private set; }

    public virtual string Resource { get; private set; }
    public virtual string CreatedBy { get; private set; }
    public virtual string CheckedBy { get; private set; }
    public virtual string CurrentOwner { get; private set; }
    public virtual DateTime CreationDate { get; private set; }

    public virtual DateTime? DesiredDate { get; private set; }

    public virtual long InternalNumber { get; private set; }

    public virtual string FormattedInternalNumber => $"AB-{InternalNumber.ToString("00000")}";

    public virtual IEnumerable<Fic> Fics => _fics?.ToList();

    public virtual IEnumerable<Document> Documents => _documents?.ToList();

    public virtual IEnumerable<Attachment> Attachments => _attachments?.ToList();

    public virtual IEnumerable<EvidenceAttachment> EvidenceAttachments => _evidenceAttachments?.ToList();

    public virtual IEnumerable<StateChangeEvent> Events => _events?.OrderBy(x => x.Date).ToList();

    internal void AddFic(Fic fic)
    {
        _fics.Add(fic);
    }

    internal void AddDocument(Document document)
    {
        _documents.Add(document);
    }

    internal void AddAttchment(Attachment attachment)
    {
        _attachments.Add(attachment);
    }

    internal void AddEvidenceAttchment(EvidenceAttachment attachment)
    {
        _evidenceAttachments.Add(attachment);
    }

    internal void AddDiscipline(Discipline discipline)
    {
        if (_disciplines.None(x => x.Discipline == discipline))
        {
            _disciplines.Add(new ChangeRequestToDiscipline(this, discipline));
        }
    }

    internal void SetComplexity(int complexity)
    {
        Complexity = complexity;
    }

    internal void SetPrioritization(GutMatrix prioritization)
    {
        Prioritization = prioritization;
    }

    internal void SetComments(string comments)
    {
        Comments = comments;
    }

    internal void SetSgmStatus(string sgmStatus)
    {
        StatusSgm = sgmStatus;
    }
}
