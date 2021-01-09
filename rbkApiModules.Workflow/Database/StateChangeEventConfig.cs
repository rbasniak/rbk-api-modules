using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Database
{
    public class StateChangeEventConfig : IEntityTypeConfiguration<StateChangeEvent>
    {
        public void Configure(EntityTypeBuilder<StateChangeEvent> entity)
        {
            entity.ToTable("StateChangeEvents");

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
                .WithMany()
                .HasForeignKey(x => x.EntityId)
                .OnDelete(DeleteBehavior.Restrict)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
