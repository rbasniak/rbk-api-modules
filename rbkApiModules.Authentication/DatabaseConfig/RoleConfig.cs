using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.ToTable("Roles");

            entity.HasIndex(x => new { x.Name, x.AuthenticationGroup }).IsUnique();

            entity.Property(x => x.Name)
                .IsRequired();
        }
    }
}
