using GCAB.Models;
using GCAB.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Diagnostics.CodeAnalysis;

namespace Demo2.Database.Config.Relational
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
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.PlatformId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.Priority)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Type)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Source)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.SourceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.Priority)
                .WithMany(x => x.ChangeRequests)
                .HasForeignKey(x => x.PriorityId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
