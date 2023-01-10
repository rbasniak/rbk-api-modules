using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class ClaimConfig : IEntityTypeConfiguration<Claim>
{
    public void Configure(EntityTypeBuilder<Claim> entity)
    {
        entity.ToTable("Claims"); 
    }
}
