using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Commons.Core.Features.ApplicationOptions;

namespace rbkApiModules.Commons.Relational;

 public class ApplicationOptionConfig : IEntityTypeConfiguration<ApplicationOption>
{
    public void Configure(EntityTypeBuilder<ApplicationOption> entity)
    {
        entity.ToTable("ApplicationOptions");

        entity.HasKey(x => x.Id);

        entity.HasIndex(x => new { x.Key, x.TenantId, x.Username }).IsUnique();

        entity.Property(x => x.Key)
              .IsRequired()
              .HasMaxLength(512);

        entity.Property(x => x.TenantId)
              .HasMaxLength(32);

        entity.Property(x => x.Username)
              .HasMaxLength(255);

        entity.Property(x => x.Value)
              .IsRequired();
    }
}

