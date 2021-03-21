using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class ManagerUserConfig : IEntityTypeConfiguration<ManagerUser>
    { 
        public void Configure(EntityTypeBuilder<ManagerUser> entity)
        {

        }
    }
}
