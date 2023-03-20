using Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Infrastructure.Database.Config;

internal class DomainEventDataObjectConfig : IEntityTypeConfiguration<DomainEventDataObject>
{
    public void Configure(EntityTypeBuilder<DomainEventDataObject> builder)
    {
        builder.ToTable("DomainEvents");
    }
}
