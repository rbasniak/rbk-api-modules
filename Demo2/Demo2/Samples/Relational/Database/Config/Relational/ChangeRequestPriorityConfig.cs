using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class ChangeRequestPriorityConfig : IEntityTypeConfiguration<ChangeRequestPriority>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestPriority> entity)
        {
            entity.ToTable("ChangeRequestPriorities");

            entity.Property(c => c.Name)
                .IsRequired();

            entity.Property(c => c.Color)
                .IsRequired();
        }
    }
}
