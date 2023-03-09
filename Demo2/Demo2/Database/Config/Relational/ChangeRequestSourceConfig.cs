using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class ChangeRequestSourceConfig : IEntityTypeConfiguration<ChangeRequestSource>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestSource> entity)
        {
            entity.ToTable("ChageRequestSources");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
