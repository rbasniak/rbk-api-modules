using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
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
