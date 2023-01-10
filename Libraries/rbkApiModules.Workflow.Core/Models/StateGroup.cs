using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class StateGroup : BaseEntity
{
    protected HashSet<State> _states;

    protected StateGroup()
    {

    }

    protected StateGroup(string name)
    {
        Name = name;

        _states = new HashSet<State>();
    }

    public virtual string Name { get; protected set; }

    public virtual IEnumerable<State> States => _states?.ToList(); 

    public virtual void Update(string name)
    {
        Name = name;
    }
}
