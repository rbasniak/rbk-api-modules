using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class State: BaseEntity
{
    protected HashSet<Transition> _transitions;
    protected HashSet<Transition> _usedBy;
    protected HashSet<BaseStateEntity> _items;

    protected State()
    {

    }

    /// <summary>
    /// Método só deve ser utilizado para testes unitários.
    /// </summary>
    protected State(Guid id, StateGroup group, string name, string systemId, string color, bool isActive): 
        this(group, name, systemId, color, isActive)
    {
        Id = id;
    }

    private State(string name, string systemId, string color, bool isActive)
    {
        _items = new HashSet<BaseStateEntity>();
        _transitions = new HashSet<Transition>();
        _usedBy = new HashSet<Transition>();

        Name = name;
        SystemId = systemId;
        Color = color;
        IsActive = isActive;
    }

    protected State(Guid groupId, string name, string systemId, string color, bool isActive)
        : this(name, systemId, color, isActive)
    {
        GroupId = groupId;
    }

    protected State(StateGroup group, string name, string systemId, string color, bool isActive)
        : this(name, systemId, color, isActive)
    {
        Group = group;
    }

    public virtual string Name { get; protected set; }

    public virtual bool IsActive { get; protected set; }

    /// <summary>
    /// Id interno do state, para ser utilizado nas partes em que as transições são hardcoded
    /// </summary>
    public virtual string SystemId { get; protected set; }

    public virtual Guid GroupId { get; protected set; }
    public virtual StateGroup Group { get; protected set; }

    /// <summary>
    /// Cor para o label no front
    /// </summary>
    public virtual string Color { get; protected set; }

    /// <summary>
    /// Flag que indica se o state pode ou não ser apagado
    /// </summary>
    public virtual bool IsProtected => !String.IsNullOrEmpty(SystemId);

    public virtual IEnumerable<Transition> Transitions => _transitions?.ToList();
    public virtual IEnumerable<Transition> UsedBy => _usedBy?.ToList();
    public virtual IEnumerable<BaseStateEntity> Items => _items?.ToList();

    public virtual void Update(string name, string color, bool isActive)
    {
        IsActive = isActive;
        Name = name;
        Color = color;
    }

    public override string ToString()
    {
        return $"{Name} [{Transitions.Count()} transitions]";
    }
}
