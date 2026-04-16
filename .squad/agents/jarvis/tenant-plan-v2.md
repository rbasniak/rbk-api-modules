# Tenant Query Filter — Revised Implementation Plan

**Date:** 2026-04-16  
**Author:** Jarvis (Backend Developer)  
**Status:** AWAITING APPROVAL

---

## 1. What Changes (and Why)

### 1.1 Add Query Filter Extension Method ✅

**Add:** `modelBuilder.ApplyRbkTenantQueryFilters(tenantProvider, config => { ... })`

**Why:** Consumers currently must manually add `.Where(x => x.TenantId == tenant)` to every query. One forgotten filter = cross-tenant data leakage (security bug). An opt-in extension method with per-entity configuration solves this while maintaining flexibility.

**Design Rationale:**
- **Opt-in by design** — Consumers call the extension explicitly in `OnModelCreating`
- **Per-entity configuration** — Each entity can have a different filter mode (TenantOnly, TenantOrGlobal, Unfiltered)
- **Runtime tenant resolution** — Uses a provider abstraction to get current tenant from HttpContext
- **No compile-time magic** — Consumer explicitly configures each entity type they want filtered

### 1.2 Make ApiKey Inherit TenantEntity ✅

**Change:** `public sealed class ApiKey : BaseEntity` → `public sealed class ApiKey : TenantEntity`

**Why:** 
- Consistency — Role inherits TenantEntity, ApiKey should too
- Eliminate duplicate TenantId property — ApiKey currently has its own `TenantId: string?` with manual normalization
- Simplify query filter logic — ApiKey would work with the same extension as Role
- Breaking but intentional — Rodrigo approved one-go breaking changes

**Impact:**
- Remove duplicate `TenantId` property from ApiKey class
- Remove `NormalizeTenantId()` helper (TenantEntity already uppercases)
- Update ApiKey EF configuration
- Consumers get inherited `HasTenant` and `HasNoTenant` properties

### 1.3 Replace TODO Comment ✅

**Change:** Remove the "Estava dando problema..." TODO and replace with clear documentation

**Current (line 14 in TenantEntity.cs):**
```csharp
// TODO: Estava dando problema no SetupTenant quando o tenantId nao era nulavel. Tem que descobrir e corrigir isso
```

**Replace with:**
```csharp
/// <summary>
/// Tenant identifier for multi-tenant isolation. NULL indicates a global entity that applies to all tenants.
/// Common for hybrid entities like Role (can be tenant-specific or application-wide) and ApiKey.
/// </summary>
```

**Why:** Nullable TenantId is intentional for hybrid entities. The TODO implies a bug when the current design is correct. The real issue was an FK constraint problem when TenantId was non-nullable and Role tried to use null (global roles).

### 1.4 Adjust SetupTenants FK Configuration ✅

**Change:** Update FK relationship to properly handle nullable TenantId

**Current:**
```csharp
builder
    .HasOne(typeof(Tenant))
    .WithMany()
    .HasForeignKey(nameof(TenantEntity.TenantId))
    .OnDelete(DeleteBehavior.Restrict);
```

**Change to:**
```csharp
builder
    .HasOne(typeof(Tenant))
    .WithMany()
    .HasForeignKey(nameof(TenantEntity.TenantId))
    .IsRequired(false)  // ✅ NEW: Allow null for global entities
    .OnDelete(DeleteBehavior.Restrict);
```

**Why:** The FK relationship must explicitly allow null values. When TenantId was non-nullable, EF Core would create a required FK constraint, which breaks when Role or ApiKey try to use null for global entities. This is the root cause mentioned in the TODO.

---

## 2. What Stays the Same (and Why)

### 2.1 TenantEntity Base Class ✅

**No Change:** `public abstract class TenantEntity : AggregateRoot` with nullable `TenantId: string?`

**Why:** The current design is correct. Nullable TenantId is intentional for hybrid entities (Role, ApiKey). No need for separate GlobalEntity/ScopedEntity base classes — the nullable TenantId already handles all three use cases:
- **Tenant-only entities** (User) — always set TenantId (enforced in constructor)
- **Hybrid entities** (Role, ApiKey) — TenantId can be set or null
- **Global entities** (Claim, Tenant) — don't inherit TenantEntity at all

Creating new base classes would add unnecessary complexity without solving any real problem.

### 2.2 User Constructor (Requires TenantId) ✅

**No Change:** User constructor requires `string tenant` parameter (non-nullable)

**Why:** Users are always tenant-scoped. The constructor enforces this at compile-time. The fact that TenantEntity.TenantId is nullable doesn't prevent individual entities from requiring it in their constructor.

### 2.3 Claim Entity (Global) ✅

**No Change:** `public sealed class Claim : BaseEntity` (no tenant awareness)

**Why:** Claims are intentionally global across all tenants. Not inheriting TenantEntity is correct.

### 2.4 Role.IsApplicationWide Property ✅

**No Change:** `public bool IsApplicationWide => String.IsNullOrEmpty(TenantId);`

**Why:** This computed property provides clear semantics for consumers. It's intentional and useful.

---

## 3. API Surface Design

### 3.1 Query Filter Extension Method

```csharp
// In: rbkApiModules.Identity.Core/Relational/Utilities/ModelBuilderExtensions.cs

/// <summary>
/// Applies tenant-based query filters to entities based on the specified configuration.
/// Must be called after SetupTenants() in OnModelCreating.
/// </summary>
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
```

### 3.2 Filter Builder API

```csharp
// In: rbkApiModules.Identity.Core/Relational/Utilities/RbkTenantFilterBuilder.cs

public sealed class RbkTenantFilterBuilder
{
    private readonly ModelBuilder _modelBuilder;
    private readonly ITenantProvider _tenantProvider;
    private readonly Dictionary<Type, TenantFilterMode> _entityFilters = new();

    internal RbkTenantFilterBuilder(ModelBuilder modelBuilder, ITenantProvider tenantProvider)
    {
        _modelBuilder = modelBuilder;
        _tenantProvider = tenantProvider;
    }

    /// <summary>
    /// Apply TenantOnly filter: WHERE TenantId = @currentTenant
    /// Use for entities that always belong to exactly one tenant (e.g., User, Post, Blog).
    /// </summary>
    public RbkTenantFilterBuilder FilterByTenantOnly<TEntity>() where TEntity : TenantEntity
    {
        _entityFilters[typeof(TEntity)] = TenantFilterMode.TenantOnly;
        return this;
    }

    /// <summary>
    /// Apply TenantOrGlobal filter: WHERE TenantId = @currentTenant OR TenantId IS NULL
    /// Use for hybrid entities that can be tenant-specific OR global (e.g., Role, ApiKey).
    /// </summary>
    public RbkTenantFilterBuilder FilterByTenantOrGlobal<TEntity>() where TEntity : TenantEntity
    {
        _entityFilters[typeof(TEntity)] = TenantFilterMode.TenantOrGlobal;
        return this;
    }

    /// <summary>
    /// Explicitly mark entity as unfiltered (no query filter applied).
    /// Use for entities that should not have tenant isolation (e.g., Claim, Tenant).
    /// </summary>
    public RbkTenantFilterBuilder NoFilter<TEntity>() where TEntity : class
    {
        _entityFilters[typeof(TEntity)] = TenantFilterMode.Unfiltered;
        return this;
    }

    internal void Apply()
    {
        foreach (var (entityType, mode) in _entityFilters)
        {
            if (mode == TenantFilterMode.Unfiltered)
                continue;

            var entityBuilder = _modelBuilder.Entity(entityType);

            if (mode == TenantFilterMode.TenantOnly)
            {
                // WHERE TenantId = @currentTenant
                var parameter = Expression.Parameter(entityType, "e");
                var property = Expression.Property(parameter, nameof(TenantEntity.TenantId));
                var tenantValue = Expression.Property(
                    Expression.Constant(_tenantProvider),
                    nameof(ITenantProvider.CurrentTenantId)
                );
                var equals = Expression.Equal(property, tenantValue);
                var lambda = Expression.Lambda(equals, parameter);

                entityBuilder.HasQueryFilter(lambda);
            }
            else if (mode == TenantFilterMode.TenantOrGlobal)
            {
                // WHERE TenantId = @currentTenant OR TenantId IS NULL
                var parameter = Expression.Parameter(entityType, "e");
                var property = Expression.Property(parameter, nameof(TenantEntity.TenantId));
                var tenantValue = Expression.Property(
                    Expression.Constant(_tenantProvider),
                    nameof(ITenantProvider.CurrentTenantId)
                );
                var equals = Expression.Equal(property, tenantValue);
                var isNull = Expression.Equal(property, Expression.Constant(null, typeof(string)));
                var orCondition = Expression.OrElse(equals, isNull);
                var lambda = Expression.Lambda(orCondition, parameter);

                entityBuilder.HasQueryFilter(lambda);
            }
        }
    }
}

public enum TenantFilterMode
{
    Unfiltered,
    TenantOnly,
    TenantOrGlobal
}
```

### 3.3 Tenant Provider Abstraction

```csharp
// In: rbkApiModules.Identity.Core/Core/Abstractions/ITenantProvider.cs

/// <summary>
/// Provides the current tenant identifier for query filter resolution.
/// Implementation should resolve from HttpContext, JWT claims, or other context.
/// </summary>
public interface ITenantProvider
{
    /// <summary>
    /// Gets the current tenant ID. Returns NULL if no tenant context is available.
    /// This property must be re-evaluated on each access (do not cache the value).
    /// </summary>
    string? CurrentTenantId { get; }
}
```

```csharp
// In: rbkApiModules.Identity.Core/Core/Services/HttpContextTenantProvider.cs

/// <summary>
/// Default implementation that resolves tenant from HttpContext.User claims.
/// </summary>
public sealed class HttpContextTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? CurrentTenantId
    {
        get
        {
            var tenant = _httpContextAccessor.HttpContext?.User.FindFirst("tenantId")?.Value;
            return string.IsNullOrEmpty(tenant) ? null : tenant.ToUpperInvariant();
        }
    }
}
```

### 3.4 Usage Example in Consumer Code

```csharp
// Consumer's DbContext

public class MyAppDbContext : DbContext
{
    private readonly ITenantProvider _tenantProvider;

    public MyAppDbContext(DbContextOptions<MyAppDbContext> options, ITenantProvider tenantProvider)
        : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Setup FK relationships
        modelBuilder.SetupTenants();

        // Apply query filters
        modelBuilder.ApplyRbkTenantQueryFilters(_tenantProvider, config =>
        {
            // User always belongs to exactly one tenant
            config.FilterByTenantOnly<User>();

            // Role can be tenant-specific OR global (application-wide)
            config.FilterByTenantOrGlobal<Role>();

            // ApiKey can be tenant-specific OR global
            config.FilterByTenantOrGlobal<ApiKey>();

            // My custom tenant-scoped entities
            config.FilterByTenantOnly<Post>();
            config.FilterByTenantOnly<Blog>();

            // Claim is global across all tenants — no filter needed
            config.NoFilter<Claim>();

            // Tenant is the master table — no filter needed
            config.NoFilter<Tenant>();
        });
    }
}
```

```csharp
// Consumer's Program.cs

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
```

---

## 4. Files to Change

### 4.1 New Files (Create)

1. **rbkApiModules.Identity.Core/Core/Abstractions/ITenantProvider.cs**
   - Interface definition for tenant resolution
   - XML documentation

2. **rbkApiModules.Identity.Core/Core/Services/HttpContextTenantProvider.cs**
   - Default implementation using HttpContext
   - Resolves from JWT claims or API key claims

3. **rbkApiModules.Identity.Core/Relational/Utilities/RbkTenantFilterBuilder.cs**
   - Fluent builder for configuring query filters
   - Expression tree construction for filters
   - Three filter modes: TenantOnly, TenantOrGlobal, Unfiltered

### 4.2 Modify Files

1. **rbkApiModules.Commons.Core/Abstractions/TenantEntity.cs**
   - Remove TODO comment (line 14)
   - Add XML documentation explaining nullable TenantId design
   - No logic changes

2. **rbkApiModules.Identity.Core/Core/Models/Entities/ApiKey.cs**
   - Change: `public sealed class ApiKey : BaseEntity` → `public sealed class ApiKey : TenantEntity`
   - Remove duplicate `TenantId` property (line 67)
   - Remove `NormalizeTenantId()` method (lines 178-186)
   - Update constructor to use inherited TenantId setter (line 40)
   - TenantEntity already uppercases, so normalization is automatic

3. **rbkApiModules.Identity.Core/Relational/Config/ApiKeyConfig.cs**
   - Remove TenantId property configuration (inherited from TenantEntity now)
   - Verify FK relationships work correctly

4. **rbkApiModules.Identity.Core/Relational/Utilities/ModelBuilderExtensions.cs**
   - Update `SetupTenants()` to add `.IsRequired(false)` on FK relationship (line 32)
   - Add new `ApplyRbkTenantQueryFilters()` extension method
   - XML documentation

### 4.3 Update Demo Projects (Breaking Change Migration)

1. **Demo1/Data/DemoDbContext.cs**
   - Add `ITenantProvider` constructor parameter
   - Add example call to `ApplyRbkTenantQueryFilters()` in `OnModelCreating`
   - Configure filters for User, Role, Post, Blog

2. **Demo2/Data/DemoDbContext.cs**
   - Same as Demo1
   - Configure filters for User, Role, ApiKey

3. **Demo1/Program.cs**
   - Add `builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();`

4. **Demo2/Program.cs**
   - Same as Demo1

5. **EF Core Migrations**
   - Generate new migration for ApiKey inheritance change
   - Demo1: `dotnet ef migrations add ApiKey_InheritsTenantEntity -p Demo1`
   - Demo2: `dotnet ef migrations add ApiKey_InheritsTenantEntity -p Demo2`
   - **NOTE:** Migration should be a no-op for database schema since TenantId already exists on ApiKey table
   - Just updates the EF model metadata

---

## 5. Breaking Changes

### 5.1 For Library Consumers

#### ApiKey Entity Model Changes (BREAKING)

**Before:**
```csharp
public sealed class ApiKey : BaseEntity
{
    public string? TenantId { get; private set; }
}
```

**After:**
```csharp
public sealed class ApiKey : TenantEntity
{
    // TenantId inherited from TenantEntity
    // HasTenant property inherited
    // HasNoTenant property inherited
}
```

**Impact:**
- Consumers using reflection or serialization on ApiKey may see new inherited properties
- Database schema unchanged (TenantId column already exists)
- EF Core navigation/configuration may need adjustment if consumer has custom ApiKey config

#### DbContext Changes (ADDITIVE — Not Breaking)

**Before:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.SetupTenants();
    // Manual query filters in each repository or service
}
```

**After (Optional but Recommended):**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.SetupTenants();

    // NEW: Opt-in query filters
    modelBuilder.ApplyRbkTenantQueryFilters(_tenantProvider, config =>
    {
        config.FilterByTenantOnly<User>();
        config.FilterByTenantOrGlobal<Role>();
        config.FilterByTenantOrGlobal<ApiKey>();
        // ... other entities
    });
}
```

**Impact:**
- **If consumer adds query filters:** Existing manual `.Where(x => x.TenantId == tenant)` clauses become redundant (but still work)
- **If consumer doesn't add query filters:** No behavior change — manual filtering still required
- Must add `ITenantProvider` to DbContext constructor
- Must register `ITenantProvider` in DI container

### 5.2 No Breaking Changes For:

- ✅ TenantEntity API surface (unchanged)
- ✅ Role entity (unchanged)
- ✅ User entity (unchanged)
- ✅ Claim entity (unchanged)
- ✅ SetupTenants() method signature (unchanged, just adds `.IsRequired(false)` internally)
- ✅ Database schema (unchanged)

### 5.3 Migration Path

1. **Update NuGet package** to new version
2. **ApiKey inheritance:**
   - If consumer has custom EF config for ApiKey.TenantId, remove it (now inherited)
   - Run `dotnet ef migrations add ApiKey_InheritsTenantEntity`
   - Review migration (should be model-only, no SQL changes)
   - Run `dotnet ef database update`
3. **Query filters (optional):**
   - Add `ITenantProvider` parameter to DbContext constructor
   - Register `HttpContextTenantProvider` in DI container
   - Call `ApplyRbkTenantQueryFilters()` in OnModelCreating
   - Test thoroughly — verify tenant isolation works correctly
   - Remove manual `.Where(x => x.TenantId == tenant)` clauses incrementally

---

## 6. What Rodrigo Needs to Decide

### ✅ Already Decided by Rodrigo:
1. Hybrid entities are intentional — keep nullable TenantId ✅
2. Query filter is opt-in and must allow per-entity configuration ✅
3. Breaking changes in one go ✅

### 🔴 Remaining Open Questions:

#### Q1: ITenantProvider Registration

**Question:** Should `ITenantProvider` be automatically registered by `AddRbkAuthentication()` or must consumers register it manually?

**Option A — Manual Registration (Recommended):**
```csharp
// Consumer's Program.cs
builder.Services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
```
- **Pros:** Consumer control, clear dependency, can provide custom implementation
- **Cons:** One more step for consumer

**Option B — Auto-Registration:**
```csharp
// Inside AddRbkAuthentication() in CoreAuthenticationBuilder.cs
services.AddScoped<ITenantProvider, HttpContextTenantProvider>();
```
- **Pros:** Less boilerplate for consumer
- **Cons:** Hidden magic, harder to replace with custom implementation

**Jarvis Recommendation:** Option A (manual). Consumer explicitly registers what they need. Clear dependency graph.

---

#### Q2: Should SetupTenants() Automatically Skip Entities With Explicit Filters?

**Context:** If consumer calls both `SetupTenants()` and `ApplyRbkTenantQueryFilters()`, should SetupTenants check for existing query filters to avoid conflicts?

**Option A — Keep Separate (Recommended):**
- SetupTenants() only sets up FK relationships
- ApplyRbkTenantQueryFilters() only sets up query filters
- Two separate concerns, called separately

**Option B — Add Detection:**
- SetupTenants() checks if entity already has a query filter
- Skip FK setup if query filter exists

**Jarvis Recommendation:** Option A. SetupTenants is about FK relationships, query filters are about runtime filtering. Separate concerns.

---

#### Q3: Should Demo Projects Show Filters or Leave Them Out?

**Question:** Should Demo1 and Demo2 call `ApplyRbkTenantQueryFilters()` or demonstrate manual filtering?

**Option A — Add Filters to Demos (Recommended):**
- Shows best practice
- Serves as example for consumers

**Option B — Leave Filters Out:**
- Shows that filters are optional
- Demonstrates manual filtering approach

**Jarvis Recommendation:** Option A. Demos should show secure defaults and best practices.

---

## Summary

**What Really Changes:**
1. ✅ Add opt-in query filter extension with per-entity configuration
2. ✅ Make ApiKey inherit TenantEntity for consistency
3. ✅ Fix SetupTenants FK to allow nullable TenantId
4. ✅ Replace TODO with clear documentation

**What Doesn't Change:**
- ✅ TenantEntity keeps nullable TenantId (intentional for hybrid entities)
- ✅ No new base classes (GlobalEntity/ScopedEntity not needed)
- ✅ Database schema unchanged
- ✅ Role, User, Claim unchanged

**Effort Estimate:**
- New files: 3 files, ~200 lines total
- Modify files: 4 files, ~50 lines changed
- Demo updates: 4 files, ~20 lines added
- Testing: 2-3 hours (verify query filters work correctly)
- **Total: 1 day implementation + testing**

**Risk Level:** LOW
- Query filter is opt-in (no automatic behavior change)
- ApiKey inheritance is clean (TenantId already exists in DB)
- Breaking changes are localized to ApiKey entity model
- No runtime behavior changes unless consumer opts in

---

**Next Steps:**
1. Rodrigo reviews and approves this plan
2. Rodrigo decides on Q1 (ITenantProvider registration)
3. Rodrigo decides on Q2 (SetupTenants behavior)
4. Rodrigo decides on Q3 (Demo project examples)
5. Jarvis implements approved plan
6. Update decisions.md with approved design
