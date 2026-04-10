using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Authentication;

public class ApiKeyToClaimConfig : IEntityTypeConfiguration<ApiKeyToClaim>
{
    public void Configure(EntityTypeBuilder<ApiKeyToClaim> entity)
    {
        entity.ToTable("ApiKeysToClaims");

        entity.HasKey(t => new { t.ClaimId, t.ApiKeyId });

        entity.HasOne(x => x.ApiKey)
            .WithMany(x => x.Claims)
            .HasForeignKey(x => x.ApiKeyId)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(x => x.Claim)
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
