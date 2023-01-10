using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class StateGroupConfig
{
    protected void Configure(EntityTypeBuilder<StateGroup> entity)
    {
        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(128);
    }
} 
