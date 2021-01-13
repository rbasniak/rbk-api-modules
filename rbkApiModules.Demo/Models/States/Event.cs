using rbkApiModules.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class Event : BaseEvent<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, ClaimToQueryDefinition>
    {
        protected Event()
        {

        }

        public Event(Guid id, string name, string systemId, bool isActive = true) : 
            base(id, name, systemId, isActive)
        {

        }

        public Event(string name, string systemId, bool isActive = true) : 
            base(name, systemId, isActive)
        {

        }
    }
}
