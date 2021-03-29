using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Payment.SqlServer
{
    public class TrialKeyConfig : IEntityTypeConfiguration<TrialKey>
    {
        public void Configure(EntityTypeBuilder<TrialKey> entity)
        {
            entity.ToTable("TrialKeys");

            entity.HasOne(x => x.Plan)
                .WithMany()
                .HasForeignKey(e => e.PlanId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
