using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Relational;

public class UserToRoleConfig : IEntityTypeConfiguration<UserToRole>
{
    public void Configure(EntityTypeBuilder<UserToRole> entity)
    {
        entity.ToTable("UsersToRoles");

        entity.HasKey(t => new { t.RoleId, t.UserId });

        entity.HasOne(uc => uc.User)
            .WithMany(u => u.Roles)
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        entity.HasOne(uc => uc.Role)
            .WithMany(c => c.Users)
            .HasForeignKey(uc => uc.RoleId)
            .OnDelete(DeleteBehavior.Restrict)
            .Metadata.PrincipalToDependent
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
