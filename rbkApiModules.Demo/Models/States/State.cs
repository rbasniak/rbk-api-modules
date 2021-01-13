using rbkApiModules.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class State: BaseState<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, ClaimToQueryDefinition>
    {
        protected State() : base()
        {

        }

        public State(StateGroup group, string name, string systemId, string color, bool isActive = true) : 
            base(group, name, systemId, color, isActive)
        {

        }

        public State(Guid groupId, string name, string systemId, string color, bool isActive = true) : 
            base(groupId, name, systemId, color, isActive)
        {

        }
    }
}
