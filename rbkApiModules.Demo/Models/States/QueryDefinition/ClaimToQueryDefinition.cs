﻿using rbkApiModules.Infrastructure.Models;
using rbkApiModules.UIAnnotations;
using rbkApiModules.Workflow;
using System;
using System.ComponentModel.DataAnnotations;

namespace rbkApiModules.Demo.Models.StateMachine
{
    public class ClaimToQueryDefinition : BaseClaimToQueryDefinition<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, ClaimToQueryDefinition>
    {
    }
}
