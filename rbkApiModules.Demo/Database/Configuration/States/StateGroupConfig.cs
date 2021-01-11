using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class StateGroupConfig: BaseStateGroupConfig, IEntityTypeConfiguration<StateGroup>
    { 
        public void Configure(EntityTypeBuilder<StateGroup> entity)
        {
            base.Configure<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup>(entity);
        }
    }
}
