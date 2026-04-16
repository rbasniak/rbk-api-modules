using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity;

/// <summary>
/// Builds EF Core query filters for tenant isolation.
/// Call in OnModelCreating via modelBuilder.ApplyRbkTenantQueryFilters().
/// </summary>
public sealed class RbkTenantFilterBuilder
{
    private readonly ModelBuilder _modelBuilder;
    private readonly ITenantProvider _tenantProvider;
    private readonly List<Action> _filterActions = new();

    internal RbkTenantFilterBuilder(ModelBuilder modelBuilder, ITenantProvider tenantProvider)
    {
        _modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
    }

    /// <summary>
    /// Filters entity to only rows matching the current tenant ID.
    /// Use for entities that are strictly tenant-scoped (e.g., User, Blog, Post).
    /// TenantId must not be null.
    /// </summary>
    public RbkTenantFilterBuilder FilterByTenantOnly<TEntity>() where TEntity : TenantEntity
    {
        _filterActions.Add(() =>
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var tenantIdProperty = Expression.Property(parameter, nameof(TenantEntity.TenantId));
            var providerConstant = Expression.Constant(_tenantProvider);
            var currentTenantIdProperty = Expression.Property(providerConstant, nameof(ITenantProvider.CurrentTenantId));
            var equalExpression = Expression.Equal(tenantIdProperty, currentTenantIdProperty);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);

            _modelBuilder.Entity<TEntity>().HasQueryFilter(lambda);
        });
        return this;
    }

    /// <summary>
    /// Filters entity to rows matching the current tenant ID OR global (null) rows.
    /// Use for hybrid entities that can be tenant-specific or application-wide (e.g., Role, ApiKey).
    /// </summary>
    public RbkTenantFilterBuilder FilterByTenantOrGlobal<TEntity>() where TEntity : TenantEntity
    {
        _filterActions.Add(() =>
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            var tenantIdProperty = Expression.Property(parameter, nameof(TenantEntity.TenantId));
            var providerConstant = Expression.Constant(_tenantProvider);
            var currentTenantIdProperty = Expression.Property(providerConstant, nameof(ITenantProvider.CurrentTenantId));

            // TenantId == CurrentTenantId OR TenantId == null
            var tenantMatches = Expression.Equal(tenantIdProperty, currentTenantIdProperty);
            var isNull = Expression.Equal(tenantIdProperty, Expression.Constant(null, typeof(string)));
            var orExpression = Expression.OrElse(tenantMatches, isNull);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(orExpression, parameter);

            _modelBuilder.Entity<TEntity>().HasQueryFilter(lambda);
        });
        return this;
    }

    /// <summary>
    /// Explicitly marks an entity as having no tenant filter.
    /// Use for non-tenant entities (e.g., Claim, Tenant) or entities with custom filtering logic.
    /// This is the default behavior — only needed for documentation clarity.
    /// </summary>
    public RbkTenantFilterBuilder NoFilter<TEntity>() where TEntity : class
    {
        // No-op — just for documentation/clarity
        return this;
    }

    /// <summary>
    /// Applies all configured filters to the model.
    /// Called automatically by ApplyRbkTenantQueryFilters() — do not call directly.
    /// </summary>
    internal void Apply()
    {
        foreach (var action in _filterActions)
        {
            action();
        }
    }
}
