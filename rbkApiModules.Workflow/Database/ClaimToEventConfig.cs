using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Workflow
{
    public class ClaimToEventConfig : IEntityTypeConfiguration<ClaimToEvent>
    {
        public void Configure(EntityTypeBuilder<ClaimToEvent> entity)
        {
            entity.ToTable("ClaimsToEvents");

            entity.HasKey(t => new { t.EventId, t.Claim });

            entity.HasOne(x => x.Event)
                .WithMany(x => x.Claims)
                .HasForeignKey(x => x.EventId)
                .OnDelete(DeleteBehavior.Restrict)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
