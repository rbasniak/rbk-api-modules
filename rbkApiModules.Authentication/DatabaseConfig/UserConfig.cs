using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public class UserConfig : IEntityTypeConfiguration<BaseUser>
    {
        public void Configure(EntityTypeBuilder<BaseUser> entity)
        {
            entity.ToTable("Users");
        }
    }
}
