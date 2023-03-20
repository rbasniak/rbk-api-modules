using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class AttachmentTypeConfig : IEntityTypeConfiguration<AttachmentType>
    {
        public void Configure(EntityTypeBuilder<AttachmentType> entity)
        {
            entity.ToTable("AttachmentTypes");

            entity.Property(c => c.Name)
                .IsRequired();
        }
    }
}
