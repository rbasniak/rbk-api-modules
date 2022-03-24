using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class QueryDefinitionToGroup : BaseQueryDefinitionToGroup<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
    }
}
