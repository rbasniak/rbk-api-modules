using Demo2.Relational;
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

            entity.HasOne(x => x.Category).WithMany();

            entity.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Fics)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
