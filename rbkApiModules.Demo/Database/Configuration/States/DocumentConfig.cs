using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;
using rbkApiModules.Demo.Models.StateMachine;
using rbkApiModules.Workflow;

namespace rbkApiModules.Demo.Database.StateMachine
{
    public class DocumentConfig : BaseStateEntityConfig, IEntityTypeConfiguration<Document>
    { 
        public void Configure(EntityTypeBuilder<Document> entity)
        {
            base.Configure<State, Event, Transition, Document, ClaimToEvent, StateChangeEvent, StateGroup>(entity);
        }
    }
}
