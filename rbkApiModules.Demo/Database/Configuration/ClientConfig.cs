﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using rbkApiModules.Demo.Models;

namespace rbkApiModules.Demo.Database
{
    public class ClientConfig: IEntityTypeConfiguration<Client>
    { 
        public void Configure(EntityTypeBuilder<Client> entity)
        {
            entity.ToTable("Clients");

            entity.Property(x => x.Name).HasMaxLength(512);

            entity.HasOne(x => x.User)
                .WithOne(x => x.Client)
                .HasForeignKey<Client>(x => x.Id);
        }
    }
}
