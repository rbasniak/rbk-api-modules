using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Workflow.Core;

namespace rbkApiModules.Workflow.Relational;

public class TransitionConfig
{
    protected void Configure(EntityTypeBuilder<Transition> entity)
    {
        entity.Property(c => c.History)
            .IsRequired()
            .HasMaxLength(512);

        entity.HasOne(x => x.Event)
            .WithMany(x => x.Transitions)
            .HasForeignKey(x => x.EventId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.ToState)
            .WithMany(x => x.UsedBy)
            .HasForeignKey(x => x.ToStateId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.FromState)
            .WithMany(x => x.Transitions)
            .HasForeignKey(x => x.FromStateId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
