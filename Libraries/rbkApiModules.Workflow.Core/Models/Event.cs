using System.Diagnostics.CodeAnalysis;
using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class Event : BaseEntity
{
    protected HashSet<Transition> _transitions;

    protected Event()
    {
        Claims = new string[0];
    }

    protected Event([NotNull] string name, string systemId, string[] claims, bool isActive)
    {
        Claims = claims;

        _transitions = new HashSet<Transition>();

        Name = name;
        SystemId = systemId;
        IsActive = isActive;
    }

    public Event(Guid id, string name, string systemId, string[] claims, bool isActive) : this(name, systemId, claims, isActive)
    {
        Id = id;
    }

    public virtual string Name { get; protected set; }

    public virtual string SystemId { get; protected set; }

    public virtual bool IsActive { get; protected set; }

    public virtual string[] Claims { get; private set; }

    public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

    public virtual IEnumerable<Transition> Transitions => _transitions?.ToList();

    public virtual void Update(string name, bool isActive)
    {
        Name = name;
        IsActive = isActive;
    }

    public override string ToString()
    {
        return Name;
    }
}
