using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class QueryDefinitionConfig
{
    protected void Configure(EntityTypeBuilder<QueryDefinition> entity)
    {
        entity.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(512);

        entity.Property(x => x.Claims)
            .HasConversion(ArrayOfStringsConverter.GetConverter(';'));
    }
}
