using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Database
{
    public class StateConfig : IEntityTypeConfiguration<State>
    {
        public void Configure(EntityTypeBuilder<State> entity)
        {
            entity.ToTable("States");

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
}
