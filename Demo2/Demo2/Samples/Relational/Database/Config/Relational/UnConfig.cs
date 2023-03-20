using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
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
