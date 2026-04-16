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
    private readonly Func<string?> _tenantIdProvider;
    private readonly List<Action> _filterActions = new();

    internal RbkTenantFilterBuilder(ModelBuilder modelBuilder, Func<string?> tenantIdProvider)
    {
        _modelBuilder = modelBuilder ?? throw new ArgumentNullException(nameof(modelBuilder));
        _tenantIdProvider = tenantIdProvider ?? throw new ArgumentNullException(nameof(tenantIdProvider));
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
            var providerConstant = Expression.Constant(_tenantIdProvider);
            var invokeMethod = typeof(Func<string?>).GetMethod("Invoke")!;
            var currentTenantIdCall = Expression.Call(providerConstant, invokeMethod);

            // When currentTenant is null (global admin or no HTTP context), show everything.
            // Otherwise: TenantId == CurrentTenantId
            var noTenantContext = Expression.Equal(currentTenantIdCall, Expression.Constant(null, typeof(string)));
            var tenantMatches = Expression.Equal(tenantIdProperty, currentTenantIdCall);
            var finalFilter = Expression.OrElse(noTenantContext, tenantMatches);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(finalFilter, parameter);

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
            var providerConstant = Expression.Constant(_tenantIdProvider);
            var invokeMethod = typeof(Func<string?>).GetMethod("Invoke")!;
            var currentTenantIdCall = Expression.Call(providerConstant, invokeMethod);

            // When currentTenant is null (global admin or unauthenticated), show everything.
            // Otherwise: TenantId == CurrentTenantId OR TenantId IS NULL
            var noTenantContext = Expression.Equal(currentTenantIdCall, Expression.Constant(null, typeof(string)));
            var tenantMatches = Expression.Equal(tenantIdProperty, currentTenantIdCall);
            var entityIsGlobal = Expression.Equal(tenantIdProperty, Expression.Constant(null, typeof(string)));
            var tenantOrGlobal = Expression.OrElse(tenantMatches, entityIsGlobal);
            var finalFilter = Expression.OrElse(noTenantContext, tenantOrGlobal);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(finalFilter, parameter);

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
