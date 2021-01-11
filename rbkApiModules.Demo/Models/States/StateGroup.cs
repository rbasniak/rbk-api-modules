﻿using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using rbkApiModules.Workflow;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class StateGroup : BaseStateGroup<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup>
    {
        protected StateGroup() : base()
        {

        }

        public StateGroup(string name): base(name)
        {

        }
    }
}
