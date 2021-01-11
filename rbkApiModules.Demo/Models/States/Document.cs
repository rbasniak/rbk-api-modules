using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using rbkApiModules.Workflow;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class Document : BaseStateEntity<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup>
    {
    }
}
