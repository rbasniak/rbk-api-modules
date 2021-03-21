using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class ClientUserConfig : IEntityTypeConfiguration<ClientUser>
    { 
        public void Configure(EntityTypeBuilder<ClientUser> entity)
        {

        }
    }
}
