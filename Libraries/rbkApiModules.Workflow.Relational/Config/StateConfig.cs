using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class StateConfig
{
    protected void Configure(EntityTypeBuilder<State> entity)
    {
        entity.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(128);

        entity.Property(c => c.SystemId)
            .HasMaxLength(128);

        entity.HasOne(x => x.Group)
            .WithMany(x => x.States)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
} 
