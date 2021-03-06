﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Authentication;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class UserConfig: BaseUserConfig, IEntityTypeConfiguration<User>
    { 
        public void Configure(EntityTypeBuilder<User> entity)
        {
            base.Configure<User>(entity, 512, 1024);

            entity.HasOne(x => x.Client)
                .WithOne(x => x.User)
                .HasForeignKey<User>(x => x.Id);
        }
    }
}
