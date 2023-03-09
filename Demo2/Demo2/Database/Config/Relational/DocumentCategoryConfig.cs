using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class DocumentCategoryConfig : IEntityTypeConfiguration<DocumentCategory>
    {
        public void Configure(EntityTypeBuilder<DocumentCategory> entity)
        {
            entity.ToTable("DocumentCategories");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
