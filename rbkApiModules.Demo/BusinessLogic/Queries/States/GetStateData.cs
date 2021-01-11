using AutoMapper;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Demo.BusinessLogic.StateMachine
{
    public class GetStateData
    {
        public class Command : BaseGetStateDataCommand
        {
        }

        public class Validator : BaseGetStateDataValidator<StateGroup>
        {
        }

        public class Handler : BaseGetStateDataHandler<Command, State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup, StateGroupDetails>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor, mapper)
            {
            }
        }
    }
}
