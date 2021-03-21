using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
{
    public class ClaimConfig : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> entity)
        {
            entity.ToTable("Claims");

            entity.HasIndex(x => new { x.Name, x.AuthenticationGroup }).IsUnique();

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Description)
                .IsRequired();
        }
    }
}
