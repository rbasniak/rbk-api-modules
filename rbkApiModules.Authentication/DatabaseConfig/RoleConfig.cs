using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable("Roles");

            entity.Property(r => r.Name)
                .IsRequired();
            // FIXME: .HasMaxLength(ModelConstants.Generic.Name.MaxLength); 
        }
    }
}
