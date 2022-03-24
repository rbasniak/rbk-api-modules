using rbkApiModules.Workflow;
using System;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class Event : BaseEvent<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
        protected Event()
        {

        }

        public Event(Guid id, string name, string systemId, string[] claims, bool isActive = true) : 
            base(id, name, systemId, claims, isActive)
        {

        }

        public Event(string name, string systemId, string[] claims, bool isActive = true) : 
            base(name, systemId, claims, isActive)
        {

        }
    }
}
