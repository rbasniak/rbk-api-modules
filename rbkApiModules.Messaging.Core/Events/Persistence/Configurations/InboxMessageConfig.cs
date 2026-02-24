// TODO: DONE, REVIEWED

// TODO: Add conditional indexes

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Commons.Core;

public class InboxMessageConfig : IEntityTypeConfiguration<InboxMessage>
{
    public void Configure(EntityTypeBuilder<InboxMessage> builder)
    {
        builder.ToTable("InboxMessages");

        builder.HasKey(x => new { x.EventId, x.HandlerName });

        builder.Property(x => x.HandlerName)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.Attempts)
            .IsRequired();

        builder.HasIndex(x => x.ProcessedUtc);
    }
} 