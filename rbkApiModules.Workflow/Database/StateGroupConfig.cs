using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow.Database
{
    public class StateGroupConfig : IEntityTypeConfiguration<StateGroup>
    {
        public void Configure(EntityTypeBuilder<StateGroup> entity)
        {
            entity.ToTable("StateGroups");

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
