using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity;

public static class ModelBuilderExtensions
{
    public static void SetupTenants(this ModelBuilder modelBuilder)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes().ToList())
        {
            var clrType = entityType.ClrType;

            // Only apply to entities that inherit from TenantEntity
            if (!typeof(TenantEntity).IsAssignableFrom(clrType))
            {
                continue;
            }

            var tenantIdProperty = clrType.GetProperty(nameof(TenantEntity.TenantId));

            if (tenantIdProperty != null && tenantIdProperty.PropertyType == typeof(string))
            {
                modelBuilder.Entity(clrType, builder =>
                {
                    builder.Property<string>(nameof(TenantEntity.TenantId));
                    builder
                        .HasOne(typeof(Tenant))
                        .WithMany()
                        .HasForeignKey(nameof(TenantEntity.TenantId))
                        .OnDelete(DeleteBehavior.Restrict);
                });
            }
        }
    }
}