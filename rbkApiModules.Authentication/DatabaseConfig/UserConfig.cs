using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> entity)
        {
            entity.HasIndex(u => u.Username).IsUnique();

            entity.Property(c => c.Username)
                .IsRequired();
            // FIXME: .HasMaxLength(ModelConstants.Authentication.Username.MaxLength);

            entity.Property(c => c.Password)
                .IsRequired();
                // FIXME: .HasMaxLength(ModelConstants.Authentication.Password.HashedLength); 
        }
    }
}
