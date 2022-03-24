using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class StateChangeEvent : BaseStateChangeEvent<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
    }
}
