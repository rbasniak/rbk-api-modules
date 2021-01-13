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
    public class GetStateDotCode
    {
        public class Command : BaseGetStatesDotCodeCommand
        {

        }

        public class Validator : BaseGetStatesDotCodeValidator<StateGroup>
        {
            public Validator(DatabaseContext context): base(context)
            {
            }
        }

        public class Handler : BaseGetStatesDotCodeHandler<Command, State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor, mapper)
            {
            }
        }
    }
}
