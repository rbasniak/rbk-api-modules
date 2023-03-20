using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class AttachmentConfig : IEntityTypeConfiguration<Attachment>
    {
        public void Configure(EntityTypeBuilder<Attachment> entity)
        {
            entity.ToTable("Attachments");

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Path);

            entity.Property(x => x.Size);

            entity.HasOne(x => x.Type)
                .WithMany();

            entity.HasOne(x => x.ChangeRequest)
                .WithMany()
                .HasForeignKey(x => x.ChangeRequestId);
        }
    }
}
