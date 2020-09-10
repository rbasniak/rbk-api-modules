using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
{
    public class ClaimConfig : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> entity)
        {
            entity.ToTable("Claims");

            entity.HasIndex(c => c.Name).IsUnique();

            entity.Property(c => c.Name)
                .IsRequired();
            // FIXME: .HasMaxLength(ModelConstants.Generic.Name.MaxLength);

            entity.Property(c => c.Description)
                .IsRequired();
            // FIXME: .HasMaxLength(ModelConstants.Generic.Name.MaxLength); 
        }
    }
}
