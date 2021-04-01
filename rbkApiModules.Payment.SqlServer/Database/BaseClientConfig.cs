using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Payment.SqlServer
{
    public abstract class BaseClientConfig
    {
        public void Configure<T>(EntityTypeBuilder<T> entity) where T : BaseClient
        { 
            entity.HasOne(x => x.Plan)
                .WithMany(nameof(Plan.Clients))
                .HasForeignKey(x => x.PlanId)
                .OnDelete(DeleteBehavior.Restrict)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.HasOne(x => x.TrialKey)
                .WithOne()
                .HasForeignKey<T>(x => x.TrialKeyId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
