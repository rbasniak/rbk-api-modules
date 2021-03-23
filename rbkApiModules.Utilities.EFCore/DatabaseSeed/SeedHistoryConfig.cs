using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.EFCore
{
    public class SeedHistoryConfig : IEntityTypeConfiguration<SeedHistory>
    {
        public void Configure(EntityTypeBuilder<SeedHistory> entity)
        {
            entity.ToTable("__SeedHistory");
        }
    }
}