using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class QueryDefinitionToStateConfig : BaseQueryDefinitionToStateConfig, IEntityTypeConfiguration<QueryDefinitionToState>
    {
        public void Configure(EntityTypeBuilder<QueryDefinitionToState> entity)
        {
            base.Configure<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>(entity);
        }
    }
}
