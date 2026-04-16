using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// Configures foreign key relationships for all TenantEntity-derived types.
    /// Call this in OnModelCreating before ApplyRbkTenantQueryFilters.
    /// </summary>
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
                        .IsRequired(false)
                        .OnDelete(DeleteBehavior.Restrict);
                });
            }
        }
    }

    /// <summary>
    /// Applies opt-in tenant query filters to entity types.
    /// Call after SetupTenants() in OnModelCreating.
    /// </summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    /// <param name="tenantProvider">The ITenantProvider instance (injected via DbContext constructor).</param>
    /// <param name="configure">Configuration callback to specify which entities get which filter mode.</param>
    public static void ApplyRbkTenantQueryFilters(
        this ModelBuilder modelBuilder,
        ITenantProvider tenantProvider,
        Action<RbkTenantFilterBuilder> configure)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(tenantProvider);
        ArgumentNullException.ThrowIfNull(configure);

        var builder = new RbkTenantFilterBuilder(modelBuilder, tenantProvider);
        configure(builder);
        builder.Apply();
    }
}