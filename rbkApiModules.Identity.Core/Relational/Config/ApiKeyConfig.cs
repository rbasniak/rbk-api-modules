using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class ApiKeyConfig : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> entity)
    {
        entity.ToTable("ApiKeys");

        entity.HasIndex(x => x.KeyHash).IsUnique();

        entity.Property(x => x.Name).IsRequired();
        entity.Property(x => x.KeyHash).IsRequired();
        entity.Property(x => x.KeyPrefix).IsRequired();
        entity.Property(x => x.RevokeReason).HasMaxLength(2048);
    }
}
