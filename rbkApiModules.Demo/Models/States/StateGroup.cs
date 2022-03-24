using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class StateGroup : BaseStateGroup<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
        protected StateGroup() : base()
        {

        }

        public StateGroup(string name): base(name)
        {

        }
    }
}
