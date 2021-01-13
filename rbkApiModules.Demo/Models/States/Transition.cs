using rbkApiModules.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class Transition : BaseTransition<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
        protected Transition()
        {

        }

        public Transition(Guid fromStateId, Guid eventId, Guid toStateId, string history, bool isProtected, bool isActive = true) : 
            base(fromStateId, eventId, toStateId, history, isProtected, isActive)
        {

        }

        public Transition(State fromState, Event @event, State toState, string history, bool isProtected, bool isActive = true) :
            base(fromState, @event, toState, history, isProtected, isActive)
        {

        }
    }
}
