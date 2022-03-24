using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class QueryDefinition : BaseQueryDefinition<
        State, 
        Event, 
        Transition, 
        Document, 
        StateChangeEvent, 
        StateGroup, 
        QueryDefinitionGroup, 
        QueryDefinition, 
        QueryDefinitionToState, 
        QueryDefinitionToGroup>
    {
    }
}
