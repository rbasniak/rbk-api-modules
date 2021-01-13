using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class TransitionConfig : BaseTransitionConfig, IEntityTypeConfiguration<Transition>
    { 
        public void Configure(EntityTypeBuilder<Transition> entity)
        {
            base.Configure<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup, ClaimToQueryDefinition>(entity);
        }
    }
}
