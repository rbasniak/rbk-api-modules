using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class ApiKeyUsageByDayConfig : IEntityTypeConfiguration<ApiKeyUsageByDay>
{
    public void Configure(EntityTypeBuilder<ApiKeyUsageByDay> entity)
    {
        entity.ToTable("ApiKeyUsageByDay");

        entity.HasKey(x => new { x.ApiKeyId, x.Date });

        entity.HasOne(x => x.ApiKey)
            .WithMany()
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
