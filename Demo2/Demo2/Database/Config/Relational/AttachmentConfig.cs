using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
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
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
