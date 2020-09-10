using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
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
