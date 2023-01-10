using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Read;

namespace Demo1.Database.Read.Config;

internal class PerformanceTest2Config : IEntityTypeConfiguration<PerformanceTest2>
{
    public void Configure(EntityTypeBuilder<PerformanceTest2> entity)
    {
        
    }
}
