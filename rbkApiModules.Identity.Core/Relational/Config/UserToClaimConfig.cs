using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class UserToClaimConfig : IEntityTypeConfiguration<UserToClaim>
{
    public void Configure(EntityTypeBuilder<UserToClaim> entity)
    {
        entity.ToTable("UsersToClaims");

        entity.HasKey(t => new { t.ClaimId, t.UserId });

        entity.HasOne(x => x.User)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.Claim)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
