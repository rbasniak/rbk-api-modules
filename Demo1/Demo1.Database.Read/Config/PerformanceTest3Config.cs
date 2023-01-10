using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Read;

namespace Demo1.Database.Read.Config;

internal class PerformanceTest3Config : IEntityTypeConfiguration<PerformanceTest3>
{
    public void Configure(EntityTypeBuilder<PerformanceTest3> entity)
    {
        
    }
}
