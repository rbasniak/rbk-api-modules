using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class EvidenceAttachmentConfig : IEntityTypeConfiguration<EvidenceAttachment>
    {
        public void Configure(EntityTypeBuilder<EvidenceAttachment> entity)
        {
            entity.ToTable("EvidenceAttachments");

            entity.Property(x => x.Name)
                .IsRequired();

            entity.Property(x => x.Path);

            entity.Property(x => x.Size);

            entity.HasOne(x => x.Type)
                .WithMany();

            entity.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.EvidenceAttachments)
                .HasForeignKey(x => x.ChangeRequestId) ;
        }
    }
}
