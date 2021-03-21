using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Authentication
{
    public abstract class BaseUserConfig
    {
        protected void Configure<T>(EntityTypeBuilder<T> entity) where T: BaseUser
        {
            entity.HasIndex(x => new { x.Username, x.AuthenticationGroup }).IsUnique();

            entity.Property(x => x.Username)
                .IsRequired();

            entity.Property(x => x.Password)
                .IsRequired(); 
        }
    }
}
