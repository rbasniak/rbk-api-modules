using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class DisciplineConfig : IEntityTypeConfiguration<Discipline>
    {
        public void Configure(EntityTypeBuilder<Discipline> entity)
        {
            entity.ToTable("Disciplines");

            entity.Property(c => c.Name)
                .IsRequired();

            entity.Metadata
                .FindNavigation(nameof(Discipline.ChangeRequests))
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
