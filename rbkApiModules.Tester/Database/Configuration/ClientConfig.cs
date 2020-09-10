using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Tester.Models;

namespace rbkApiModules.Tester.Database
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
