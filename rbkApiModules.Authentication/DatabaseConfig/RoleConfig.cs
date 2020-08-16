using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> entity)
        {
            entity.Property(r => r.Name)
                .IsRequired();
            // FIXME: .HasMaxLength(ModelConstants.Generic.Name.MaxLength); 
        }
    }
}
