using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Demo1.Models.Domain;

namespace Demo1.Database.Domain;

internal class PlantConfig : IEntityTypeConfiguration<Plant>
{
    public void Configure(EntityTypeBuilder<Plant> entity)
    {
    }
}