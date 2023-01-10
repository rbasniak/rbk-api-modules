using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class QueryDefinition : BaseEntity
{
    private HashSet<QueryDefinitionToState> _filteringStates;
    private HashSet<QueryDefinitionToGroup> _groups;

    protected QueryDefinition()
    {
        Claims = new string[0];
    }

    public QueryDefinition(string name, string description, string menu, State[] filteringStates, bool isActive, bool listOnlyUserItems, bool editableItems, string[] editableFields)
    {
        Claims = new string[0];

        _filteringStates = new HashSet<QueryDefinitionToState>();
        _groups = new HashSet<QueryDefinitionToGroup>();

        Update(name, description, filteringStates, isActive);
    }

    public virtual string Name { get; private set; }

    public virtual string Description { get; private set; }

    public virtual bool IsActive { get; private set; }

    public virtual string[] Claims { get; private set; }

    public virtual IEnumerable<QueryDefinitionToState> FilteringStates => _filteringStates?.ToList();

    public virtual IEnumerable<QueryDefinitionToGroup> Groups => _groups?.ToList();

    public virtual void Update(string name, string description, State[] filteringStates, bool isActive)
    {
        // TODO: Fazer o delta para permitir atualizar os states de  filtragem
        if (_filteringStates.Count > 0) throw new ApplicationException("Não é possível atualizar os status de filtragem.");

        IsActive = isActive;
        Name = name;
        Description = description;

        foreach (var state in filteringStates)
        {
            var queryDefinition = new QueryDefinitionToState(this, state);
            _filteringStates.Add(queryDefinition);
        }
    }

    public override string ToString()
    {
        return $"{Name}";
    }
}
