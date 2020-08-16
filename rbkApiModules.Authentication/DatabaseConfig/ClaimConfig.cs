using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public class ClaimConfig : IEntityTypeConfiguration<Claim>
    {
        public void Configure(EntityTypeBuilder<Claim> entity)
        {
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
