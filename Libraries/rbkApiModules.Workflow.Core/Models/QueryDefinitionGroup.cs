using rbkApiModules.Commons.Core;

namespace rbkApiModules.Workflow.Core;

public class QueryDefinitionGroup : BaseEntity
{
    private HashSet<QueryDefinitionToGroup> _queries;

    protected QueryDefinitionGroup()
    {

    }

    public QueryDefinitionGroup(string name, string description)
    {
        Name = name;
        Description = description;

        _queries = new HashSet<QueryDefinitionToGroup>();
    }

    public virtual string Name { get; private set; }

    public virtual string Description { get; private set; }

    public virtual IEnumerable<QueryDefinitionToGroup> Queries => _queries?.ToList();
}
