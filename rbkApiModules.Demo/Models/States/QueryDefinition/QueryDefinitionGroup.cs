using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class QueryDefinitionGroup : BaseQueryDefinitionGroup<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
    }
}
