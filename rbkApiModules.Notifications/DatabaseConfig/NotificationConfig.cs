
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace rbkApiModules.Notifications
{
    public class NotificationConfig : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> entity)
        {
            entity.ToTable("Notifications");

            entity.Property(c => c.Category)
                .HasMaxLength(255);

            entity.Property(c => c.Title)
                .HasMaxLength(255);

            entity.Property(c => c.Body)
                .HasMaxLength(1024);

            entity.Property(c => c.Link)
                .HasMaxLength(255);

            entity.Property(c => c.Route)
                .HasMaxLength(255);

            entity.Property(c => c.User)
                .HasMaxLength(64);
        }
    }
}
