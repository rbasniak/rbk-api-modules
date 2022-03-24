using AutoMapper;
using Microsoft.AspNetCore.Http;
using rbkApiModules.Demo.Database;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.BusinessLogic.StateMachine
{
    public class GetQueryDefinitionResults
    {
        public class Command : BaseGetQueryDefinitionResultsCommand
        {
        }

        public class Validator : BaseGetQueryDefinitionResultsValidator<QueryDefinition>
        {
            public Validator(DatabaseContext context): base(context)
            {

            }
        }

        public class Handler : BaseGetQueryDefinitionResultsHandler<Command, State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, StateGroupDetails>
        {
            public Handler(DatabaseContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper) 
                : base(context, httpContextAccessor, mapper)
            {
            }
        }
    }
}
