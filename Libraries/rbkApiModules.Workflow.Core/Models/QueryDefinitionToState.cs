namespace rbkApiModules.Workflow.Core;

public class QueryDefinitionToState 
{
    protected QueryDefinitionToState()
    {

    }

    public QueryDefinitionToState(QueryDefinition query, State state)
    {
        State = state;
        Query = query;
    }

    public virtual Guid QueryId { get; private set; }
    public virtual QueryDefinition Query { get; private set; }

    public virtual Guid StateId { get; private set; }
    public virtual State State { get; private set; }

    public void SetKeys(QueryDefinition query, State state)
    {
        State = state;
        Query = query;
    }
}
