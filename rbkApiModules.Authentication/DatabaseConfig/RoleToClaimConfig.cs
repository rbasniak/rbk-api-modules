using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;

namespace AspNetCoreApiTemplate.Database
{
    public class RoleToClaimConfig : IEntityTypeConfiguration<RoleToClaim>
    {
        public void Configure(EntityTypeBuilder<RoleToClaim> entity)
        {
            entity.ToTable("RolesToClaims");

            entity.HasKey(rc => new { rc.ClaimId, rc.RoleId });

            entity.HasOne(rc => rc.Claim)
                .WithMany(c => c.Roles)
                .HasForeignKey(rc => rc.ClaimId)
                .OnDelete(DeleteBehavior.Restrict)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            entity.HasOne(rc => rc.Role)
                .WithMany(r => r.Claims)
                .HasForeignKey(rc => rc.RoleId)
                .OnDelete(DeleteBehavior.Restrict)
                .Metadata.PrincipalToDependent
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
