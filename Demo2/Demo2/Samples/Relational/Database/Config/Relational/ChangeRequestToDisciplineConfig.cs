using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    [ExcludeFromCodeCoverage]
    public class ChangeRequestToDisciplineConfig : IEntityTypeConfiguration<ChangeRequestToDiscipline>
    {
        public void Configure(EntityTypeBuilder<ChangeRequestToDiscipline> entity)
        {
            entity.ToTable("ChangeRequestToDisciplines");

            entity.HasKey(x => new { x.ChangeRequestId, x.DisciplineId });

            entity.HasOne(x => x.ChangeRequest).WithMany().HasForeignKey(x => x.ChangeRequestId);


            entity.HasOne(x => x.Discipline).WithMany().HasForeignKey(x => x.DisciplineId);
        }
    }
}
