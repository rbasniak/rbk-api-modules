using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class QueryDefinitionToGroupConfig : BaseQueryDefinitionToGroupConfig, IEntityTypeConfiguration<QueryDefinitionToGroup>
    {
        public void Configure(EntityTypeBuilder<QueryDefinitionToGroup> entity)
        {
            base.Configure<State, Event, Transition, Document, StateChangeEvent, StateGroup, QueryDefinitionGroup, QueryDefinition, QueryDefinitionToState, QueryDefinitionToGroup>(entity);
        }
    }
}
