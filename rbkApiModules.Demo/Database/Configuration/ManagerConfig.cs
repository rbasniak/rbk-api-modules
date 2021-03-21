using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class ManagerConfig: IEntityTypeConfiguration<Manager>
    { 
        public void Configure(EntityTypeBuilder<Manager> entity)
        {
            entity.ToTable("Managers");

            entity.Property(x => x.Name).HasMaxLength(512);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Manager)
                .HasForeignKey<Manager>(x => x.Id);
        }
    }
}
