using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models;
using rbkApiModules.Payment.SqlServer;

namespace rbkApiModules.Demo.Database
{
    public class ClientConfig : BaseClientConfig, IEntityTypeConfiguration<Client>
    { 
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            Configure<Client>(entity);

            entity.Property(x => x.Name).HasMaxLength(512);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Client)
                .HasForeignKey<Client>(x => x.Id);
        }
    }
}
