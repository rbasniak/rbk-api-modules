using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class PlatformConfig : IEntityTypeConfiguration<Platform>
    {
        public void Configure(EntityTypeBuilder<Platform> entity)
        {
            entity.ToTable("Platforms");

            entity.HasOne(platform => platform.Un)
              .WithMany(un => un.Platforms)
              .HasForeignKey(platform => platform.UnId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
