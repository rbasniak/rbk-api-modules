using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public abstract class BaseUserConfig
    {
        protected void Configure<T>(EntityTypeBuilder<T> entity, int usernameMaxLenght, int hashedPasswordMaxLength) where T: BaseUser
        {
            entity.HasIndex(u => u.Username).IsUnique();

            entity.Property(c => c.Username)
                .IsRequired()
                .HasMaxLength(usernameMaxLenght);

            entity.Property(c => c.Password)
                .IsRequired()
                .HasMaxLength(hashedPasswordMaxLength); 
        }
    }
}
