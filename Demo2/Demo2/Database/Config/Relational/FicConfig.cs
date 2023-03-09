using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class FicConfig : IEntityTypeConfiguration<Fic>
    {
        public void Configure(EntityTypeBuilder<Fic> entity)
        {
            entity.ToTable("Fics");

            entity.Property(c => c.Name)
                .IsRequired();

            entity.HasOne(x => x.Category)
                .WithMany(x => x.Fics)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Fics)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
