using AspNetCoreApiTemplate.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Tester.Models; 

namespace rbkApiModules.Tester.Database.Configuration
{
    public class ClientConfig: IEntityTypeConfiguration<Client>
    { 
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            entity.ToTable("Clients");

            entity.Property(x => x.Name).HasMaxLength(512);
        }
    }
}
