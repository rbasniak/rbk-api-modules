using rbkApiModules.Workflow;
using System;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class Document : BaseStateEntity<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
        protected override StateChangeEvent CreateStateChangeEvent(string user, Transition transition)
        {
            throw new NotImplementedException();
        }
    }
}
