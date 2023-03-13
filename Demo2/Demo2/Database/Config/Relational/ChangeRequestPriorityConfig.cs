using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
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
