using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class FicCategoryConfig : IEntityTypeConfiguration<FicCategory>
    {
        public void Configure(EntityTypeBuilder<FicCategory> entity)
        {
            entity.ToTable("FicCategories");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
