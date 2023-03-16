using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Reflection;

namespace rbkApiModules.Authentication;

public class UserConfig : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.ToTable("Users");

        entity.OwnsOne(typeof(PasswordRedefineCode), nameof(User.PasswordRedefineCode));
    }
}

