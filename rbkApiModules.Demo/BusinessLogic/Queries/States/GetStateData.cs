using AutoMapper;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.BusinessLogic.StateMachine
{
    public class GetStateData
    {
        public class Command : BaseGetStateDataCommand
        {
        }

        public class Validator : BaseGetStateDataValidator
        {
        }

        public class Handler : BaseGetStateDataHandler<Command, State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, StateGroupDetails>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor, mapper)
            {
            }
        }
    }
}
