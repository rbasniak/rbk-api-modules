using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class StateChangeEventConfig
{
    protected void Configure(EntityTypeBuilder<StateChangeEvent> entity)
    {
        entity.Property(c => c.Notes)
            .IsRequired()
            .HasMaxLength(2048);

        entity.Property(c => c.StatusHistory)
            .IsRequired()
            .HasMaxLength(1024);

        entity.Property(c => c.StatusName)
            .IsRequired()
            .HasMaxLength(128);

        entity.Property(c => c.Username)
            .IsRequired()
            .HasMaxLength(255);

        entity.HasOne(x => x.Entity)
            .WithMany(x => x.Events)
            .HasForeignKey(x => x.EntityId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
} 
