using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using rbkApiModules.Workflow;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class StateChangeEvent : BaseStateChangeEvent<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
    {
    }
}
