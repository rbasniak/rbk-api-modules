using rbkApiModules.Commons.Core;
using System;

namespace Demo2.Relational;

public class StateChangeEvent : BaseEntity
{
    public StateChangeEvent()
    {

    }

    public StateChangeEvent(ChangeRequest changeRequest, string user, State status, string history, string notes, string newOwner)
    {
        ChangeRequest = changeRequest;
        User = user;
        Date = DateTime.Now;
        StatusHistory = history;
        StatusId = status.Id;
        StatusName = status.Name;
        Notes = notes;
        NewOwner = newOwner;
    }

    public string StatusName { get; private set; }

    public Guid StatusId { get; private set; }

    public string StatusHistory { get; private set; }

    public virtual string User { get; private set; }

    public virtual string NewOwner { get; private set; }

    public DateTime Date { get; private set; }

    public Guid ChangeRequestId { get; private set; }
    public virtual ChangeRequest ChangeRequest { get; private set; }

    public string Notes { get; private set; }
}
