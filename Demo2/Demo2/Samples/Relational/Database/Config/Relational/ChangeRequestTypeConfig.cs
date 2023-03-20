using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class ChangeRequestTypeConfig : IEntityTypeConfiguration<ChangeRequestType>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestType> entity)
        {
            entity.ToTable("ChangeRequestTypes");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
