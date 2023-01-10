using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class EventConfig
{
    protected void Configure(EntityTypeBuilder<Event> entity)
    {
        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(1024);

        entity.Property(x => x.Claims)
            .HasConversion(ArrayOfStringsConverter.GetConverter(';'));
    }
}
