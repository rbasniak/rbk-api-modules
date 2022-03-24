using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class QueryDefinitionToState : BaseQueryDefinitionToState<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
    }
}
