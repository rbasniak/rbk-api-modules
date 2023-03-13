using Demo2.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class ChangeRequestToDisciplineConfig : IEntityTypeConfiguration<ChangeRequestToDiscipline>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestToDiscipline> entity)
        {
            entity.ToTable("ChangeRequestToDisciplines");

            entity.HasKey(x => new { x.ChangeRequestId, x.DisciplineId });

            entity.HasOne(x => x.ChangeRequest)
                .WithMany(x => x.Disciplines)
                .HasForeignKey(x => x.ChangeRequestId)
                .OnDelete(DeleteBehavior.Cascade);


            entity.HasOne(x => x.Discipline)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.DisciplineId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
