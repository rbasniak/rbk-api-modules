using Demo2.Samples.Relational.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Samples.Relational.Database.Config.Relational
{
    public class ChangeRequestConfig : IEntityTypeConfiguration<ChangeRequest>
    {
        [ExcludeFromCodeCoverage]
        public void Configure(EntityTypeBuilder<ChangeRequest> entity)
        {
            entity.ToTable("ChangeRequests");

            entity.Property(c => c.CheckedBy);

            entity.Property(c => c.Resource);

            entity.Property(c => c.CreatedBy)
                .IsRequired();

            entity.Property(c => c.InternalNumber)
                .ValueGeneratedOnAdd();

            entity.OwnsOne(x => x.Prioritization);

            entity.HasOne(x => x.State)
                .WithMany()
                .HasForeignKey(x => x.StateId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Platform)
                .WithMany();

            entity.HasOne(x => x.Priority)
                .WithMany();

            entity.HasOne(x => x.Type)
                .WithMany();

            entity.HasOne(x => x.Source)
                .WithMany();

            entity.HasOne(x => x.Priority)
                .WithMany();

        }
    }
}
