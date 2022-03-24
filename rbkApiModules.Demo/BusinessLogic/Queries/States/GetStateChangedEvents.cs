using AutoMapper;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.BusinessLogic.StateMachine
{
    public class GetStateChangedEvents
    {
        public class Command : BaseGetStateChangedEventsCommand
        {
        }

        public class Validator : BaseGetStateChangedEventsValidator<StateChangeEvent>
        {
            public Validator(DatabaseContext context): base(context)
            {

            }
        }

        public class Handler : BaseGetStateChangedEventsHandler<Command, State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, StateChangedEventDetails>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor, mapper)
            {
            }
        }
    }
}
