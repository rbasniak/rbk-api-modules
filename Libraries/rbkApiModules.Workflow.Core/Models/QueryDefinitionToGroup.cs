namespace rbkApiModules.Workflow.Core;

public class QueryDefinitionToGroup 
{
    protected QueryDefinitionToGroup()
    {

    }

    protected QueryDefinitionToGroup(QueryDefinition query, QueryDefinitionGroup group)
    {
        Group = group;
        Query = query;
    }

    public virtual Guid QueryId { get; private set; }
    public virtual QueryDefinition Query { get; private set; }

    public virtual Guid GroupId { get; private set; }
    public virtual QueryDefinitionGroup Group { get; private set; }
}
