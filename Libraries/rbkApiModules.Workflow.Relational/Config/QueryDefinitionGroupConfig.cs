using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class QueryDefinitionGroupConfig
{
    protected void Configure(EntityTypeBuilder<QueryDefinitionGroup> entity)
    {
        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255); 
    } 
} 
