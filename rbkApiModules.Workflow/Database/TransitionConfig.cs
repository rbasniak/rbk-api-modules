using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Database
{
    public class TransitionConfig : IEntityTypeConfiguration<Transition>
    {
        public void Configure(EntityTypeBuilder<Transition> entity)
        {
            entity.ToTable("Transitions");

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
}
