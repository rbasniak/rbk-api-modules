using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class UnConfig : IEntityTypeConfiguration<Un>
    {
        public void Configure(EntityTypeBuilder<Un> entity)
        {
            entity.ToTable("Uns");

            entity.Property(c => c.Name)
                .IsRequired();

            entity.Property(c => c.Description)
                .IsRequired();
        }
    }
}
