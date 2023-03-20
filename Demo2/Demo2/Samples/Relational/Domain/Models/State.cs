using rbkApiModules.Commons.Core;

namespace Demo2.Samples.Relational.Domain.Models;

public class State : BaseEntity
{
    private HashSet<ChangeRequest> _items;

    public State()
    {

    }
    public State(string name)
    {
        _items = new HashSet<ChangeRequest>();

        Name = name;
    }

    public virtual string Name { get; private set; }
}
