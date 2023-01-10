using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Commons.Relational;

public class SeedHistoryConfig : IEntityTypeConfiguration<SeedHistory>
{
    public void Configure(EntityTypeBuilder<SeedHistory> entity)
    {
        entity.ToTable("__SeedHistory");
    }
}