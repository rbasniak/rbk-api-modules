using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class UserToClaimConfig : IEntityTypeConfiguration<UserToClaim>
{
    public void Configure(EntityTypeBuilder<UserToClaim> entity)
    {
        entity.ToTable("UsersToClaims");

        entity.HasKey(t => new { t.ClaimId, t.UserId });

        entity.HasOne(uc => uc.User)
            .WithMany(u => u.Claims)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(uc => uc.Claim)
            .WithMany(c => c.Users)
            .HasForeignKey(uc => uc.ClaimId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
