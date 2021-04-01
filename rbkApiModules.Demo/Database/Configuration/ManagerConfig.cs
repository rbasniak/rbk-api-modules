using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models;
using rbkApiModules.Payment.SqlServer;

namespace rbkApiModules.Demo.Database
{
    public class ManagerConfig : BaseClientConfig, IEntityTypeConfiguration<Manager>
    { 
        public void Configure(EntityTypeBuilder<Manager> entity)
        {
            Configure<Manager>(entity);

            entity.Property(x => x.Name).HasMaxLength(512);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Manager)
                .HasForeignKey<Manager>(x => x.Id);
        }
    }
}
