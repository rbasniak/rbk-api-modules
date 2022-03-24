using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class StateChangeEventConfig : BaseStateChangeEventConfig, IEntityTypeConfiguration<StateChangeEvent>
    {
        public void Configure(EntityTypeBuilder<StateChangeEvent> entity)
        {
            base.Configure<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>(entity);
        }
    }
}
