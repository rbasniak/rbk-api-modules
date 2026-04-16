# Tenant Query Filter Security Bug — Technical Investigation

**Investigation Date:** 2026-04-16  
**Investigator:** Jarvis (Backend Developer)  
**Request:** Rodrigo Basniak

---

## A. Current State

### Class Hierarchy

```
BaseEntity (abstract)
    ├─ Id: Guid
    └─ (no tenant awareness)

AggregateRoot : BaseEntity (abstract)
    └─ Domain events management

TenantEntity : AggregateRoot (abstract)
    ├─ TenantId: string? (nullable, MaxLength 255)
    ├─ HasTenant: bool (computed property)
    └─ HasNoTenant: bool (computed property)
```

**Key Implementation Details:**
- `TenantId` is **nullable** (`string?`)
- `TenantId` setter automatically converts to uppercase: `_tenantId = value.ToUpper()`
- `HasTenant` returns `!String.IsNullOrEmpty(TenantId)`
- `HasNoTenant` returns `String.IsNullOrEmpty(TenantId)`

### Current TenantId Type and Nullability

**Type:** `string?` (nullable reference type)  
**Validation:** `[MaxLength(255)]`  
**Behavior:** Auto-uppercases on set

**The TODO Comment (Line 14):**
```csharp
// TODO: Estava dando problema no SetupTenant quando o tenantId nao era nulavel. 
// Tem que descobrir e corrigir isso
```

**Translation:** "Was having problems with SetupTenant when tenantId was not nullable. Need to find and fix this."

### Entities Using TenantEntity

**In rbkApiModules.Identity.Core:**
1. **User** : TenantEntity (sealed)
   - Multi-tenant authentication entity
   - Constructor requires `string tenant` parameter
   - Used for JWT authentication with tenant claims

2. **Role** : TenantEntity (sealed)
   - Multi-tenant role entity
   - Has `IsApplicationWide` property = `String.IsNullOrEmpty(TenantId)`
   - Can be tenant-scoped OR application-wide (null TenantId)

**In rbkApiModules.Identity.Core (NOT inheriting TenantEntity):**
3. **ApiKey** : BaseEntity (sealed)
   - **Does NOT inherit TenantEntity**
   - Has its own `TenantId: string?` property
   - Manually normalizes TenantId: `NormalizeTenantId(tenantId)` → `ToUpperInvariant()`
   - Can be tenant-scoped OR application-wide

4. **Claim** : BaseEntity (sealed)
   - **Global entity** - no tenant isolation
   - Claims are shared across all tenants

5. **Tenant** : **No base class**
   - Stands alone
   - `Alias: string` (PK, uppercased)
   - The master tenant registry table

**In Demo1:**
1. **Post** : TenantEntity
2. **Blog** : TenantEntity
3. **Plant** : TenantEntity
4. **Author** : BaseEntity (NOT tenant-scoped)

**In Demo2:**
- No custom entities (uses Identity.Core entities only)

### SetupTenants Extension

**Location:** `rbkApiModules.Identity.Core\Relational\Utilities\ModelBuilderExtensions.cs`

```csharp
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
```

**What it does:**
- Automatically creates a **foreign key relationship** from `TenantEntity.TenantId` → `Tenant.Alias`
- Applies to all entities inheriting from `TenantEntity`
- Uses `DeleteBehavior.Restrict` to prevent cascade deletes
- **CRITICALLY: Does NOT apply any query filters**

**Called in:**
- `Demo1\Database\DatabaseContext.cs:40` → `modelBuilder.SetupTenants();`
- `Demo2\Database\DatabaseContext.cs:35` → `modelBuilder.SetupTenants();`

### Query Filters Today

**Status:** ❌ **NONE EXIST**

**Evidence:**
- `grep` search for `HasQueryFilter` found **ZERO** results in production code
- Only reference is in `.squad\agents\stark\architecture-analysis.md` (documentation):
  > "Consumers must manually apply `modelBuilder.Entity<TenantEntity>().HasQueryFilter(x => x.TenantId == tenantId)`"

**Implication:**  
**WITHOUT query filters, ALL queries return data from ALL tenants unless developers manually add `.Where(x => x.TenantId == tenant)` to EVERY query.**

### How Developers Query Tenant Data Today

**Pattern observed in `rbkApiModules.Identity.Core`:**

Every query manually filters by TenantId:

```csharp
// From RelationalAuthService.cs:52
.SingleAsync(x => (x.Username.ToLower() == username.ToLower() || x.Email.ToLower() == username) 
    && x.TenantId == tenant, cancellationToken);

// From RelationalRolesService.cs:42
.Where(x => x.TenantId == tenant).ToListAsync();

// From ApiKeyAuthorization.cs:31
return query.Where(x => x.TenantId == tenant);
```

**Test pattern in Demo1.Tests:**
```csharp
var users = TestingServer.CreateContext().Set<User>()
    .Where(x => x.TenantId == "WAYNE INC").ToList();
```

**This is MANUAL and ERROR-PRONE.** A single forgotten `.Where()` clause leaks all tenant data.

---

## B. The Bug / Gap

### What the TODO Actually Warns About

The TODO reveals that:
1. At some point, an attempt was made to make `TenantId` **non-nullable** (`string`)
2. This broke `SetupTenants()` (the foreign key configuration)
3. The fix was to make it **nullable again** (`string?`)
4. The root cause was **never investigated or resolved**

### The Real Security Risk

**There is NO automatic tenant isolation today.**

#### Concrete Security Risk:

**Scenario 1: Developer forgets `.Where()` clause**
```csharp
// BUG: Returns ALL users from ALL tenants
var users = await _context.Set<User>().ToListAsync();

// CORRECT: Returns users from specific tenant only
var users = await _context.Set<User>()
    .Where(x => x.TenantId == currentTenant)
    .ToListAsync();
```

**Scenario 2: EF Core navigation properties**
```csharp
var user = await _context.Set<User>()
    .Include(x => x.Roles)  // ← Roles might include other tenants' data!
    .FirstAsync(x => x.Username == "admin");
```

**Scenario 3: New developer joins the team**
- They write a query
- They don't know about manual tenant filtering
- **Production data leakage occurs**

**Scenario 4: ApiKey entity inconsistency**
- ApiKey doesn't inherit TenantEntity
- Has its own TenantId property
- No FK relationship to Tenant table
- Different normalization logic

#### Is This a "Bug" or a "Design Gap"?

**It's a DESIGN GAP with SECURITY IMPACT:**
- The library **provides** TenantEntity base class
- The library **provides** SetupTenants() to configure FK relationships
- The library **does NOT provide** automatic query filtering
- The library **does NOT provide** a helper to apply filters
- Documentation **does NOT warn** developers about manual filtering requirement

**Result:** Developers using rbkApiModules are ONE FORGOTTEN WHERE CLAUSE away from cross-tenant data leakage.

### Why TenantId is Nullable Today

**Hypothesis based on code analysis:**

1. **Dual-purpose entities** — Role is designed to be EITHER tenant-scoped OR application-wide:
   ```csharp
   public bool IsApplicationWide => String.IsNullOrEmpty(TenantId);
   ```

2. **FK constraint issue** — If TenantId is non-nullable:
   - Application-wide roles can't have `TenantId = null`
   - FK to `Tenant.Alias` would fail
   - SetupTenants() would try to create a required FK
   - No "null" tenant exists in the Tenant table

3. **EF Core initialization** — Private parameterless constructors set `TenantId = null`:
   ```csharp
   private User() { /* EF Core constructor */ }
   ```

**The original developer hit this constraint issue and made TenantId nullable as a workaround without solving the underlying architectural problem.**

---

## C. Options for the Fix

### Option 1: Add `modelBuilder.ApplyRbkTenantQueryFilters()` Extension

**Shape:**
```csharp
namespace rbkApiModules.Commons.Relational;

public static class TenantQueryFilterExtensions
{
    public static void ApplyRbkTenantQueryFilters(
        this ModelBuilder modelBuilder, 
        Func<string?> tenantProvider)
    {
        ArgumentNullException.ThrowIfNull(modelBuilder);
        ArgumentNullException.ThrowIfNull(tenantProvider);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (!typeof(TenantEntity).IsAssignableFrom(entityType.ClrType))
                continue;

            var parameter = Expression.Parameter(entityType.ClrType, "e");
            var property = Expression.Property(parameter, nameof(TenantEntity.TenantId));
            var tenantValue = Expression.Constant(tenantProvider(), typeof(string));
            var equals = Expression.Equal(property, tenantValue);
            var lambda = Expression.Lambda(equals, parameter);

            modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
        }
    }
}
```

**Usage:**
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    var httpContextAccessor = /* resolve from DI */;
    
    modelBuilder.ApplyRbkTenantQueryFilters(() => 
    {
        var user = httpContextAccessor.HttpContext?.User;
        return user?.FindFirst("tenant")?.Value;
    });
}
```

**Tradeoffs:**
- ✅ **Additive** — no breaking changes
- ✅ **Opt-in** — consumers choose to enable it
- ✅ **Automatic** — once enabled, all queries filtered
- ❌ **DI complexity** — DbContext needs access to IHttpContextAccessor
- ❌ **Global state** — query filters use runtime context
- ❌ **Testing friction** — must mock tenant context in tests
- ❌ **Doesn't address nullable TenantId design**

**Problem:** Still allows `TenantId = null` entities, which would be excluded from ALL tenant queries. Is that intentional?

---

### Option 2: Override SaveChanges to Enforce TenantId (Write-Side)

**Shape:**
```csharp
public abstract class TenantAwareDbContext : DbContext
{
    private readonly Func<string?> _tenantProvider;

    protected TenantAwareDbContext(
        DbContextOptions options, 
        Func<string?> tenantProvider) : base(options)
    {
        _tenantProvider = tenantProvider;
    }

    public override int SaveChanges()
    {
        EnforceTenantId();
        return base.SaveChanges();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        EnforceTenantId();
        return await base.SaveChangesAsync(ct);
    }

    private void EnforceTenantId()
    {
        var currentTenant = _tenantProvider();
        
        foreach (var entry in ChangeTracker.Entries<TenantEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                if (string.IsNullOrEmpty(entry.Entity.TenantId))
                {
                    entry.Entity.TenantId = currentTenant;
                }
                else if (entry.Entity.TenantId != currentTenant)
                {
                    throw new InvalidOperationException(
                        $"Cannot save entity to tenant '{entry.Entity.TenantId}' " +
                        $"while authenticated as tenant '{currentTenant}'");
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                // Prevent TenantId changes
                if (entry.Property(nameof(TenantEntity.TenantId)).IsModified)
                {
                    throw new InvalidOperationException(
                        "TenantId cannot be changed after entity creation");
                }
            }
        }
    }
}
```

**Tradeoffs:**
- ✅ **Write protection** — prevents cross-tenant writes
- ✅ **Auto-assignment** — developers don't need to set TenantId manually
- ❌ **Doesn't solve read-side** — queries still leak without filters
- ❌ **Requires base class** — breaking change (DbContext → TenantAwareDbContext)
- ❌ **Partial solution** — need BOTH this AND query filters

**Verdict:** Good defense-in-depth, but NOT sufficient on its own.

---

### Option 3: Make TenantId Non-Nullable (Breaking Change)

**Shape:**
```csharp
public abstract class TenantEntity : AggregateRoot
{
    [Required, MaxLength(255)]
    public string TenantId { get; private set; } = string.Empty;
    
    protected void SetTenantId(string tenantId)
    {
        if (string.IsNullOrEmpty(TenantId))
        {
            TenantId = tenantId.ToUpper();
        }
        else
        {
            throw new InvalidOperationException("TenantId is immutable");
        }
    }
}
```

**What breaks:**
1. **Role.IsApplicationWide pattern** — application roles can't have `TenantId = null`
2. **EF Core FK constraint** — SetupTenants() would enforce required FK
3. **All existing migrations** — TenantId column must become NOT NULL
4. **Existing code** — 50+ places check `TenantId == null`

**What becomes simpler:**
- Type safety: `string` vs `string?`
- Query filters: no null checks needed
- Clearer intent: "This entity MUST belong to a tenant"

**Required follow-up work:**
1. Create separate `GlobalEntity : AggregateRoot` for application-wide entities
2. Refactor Role, ApiKey to use GlobalEntity or add separate TenantId column
3. Migration to backfill null TenantId values
4. Update all null checks

**Effort:** 2-3 days (high risk, requires migration strategy)

---

### Option 4: Two Base Classes (TenantEntity + GlobalEntity) — CLEAN SEPARATION

**Shape:**
```csharp
// For tenant-scoped entities
public abstract class TenantEntity : AggregateRoot
{
    [Required, MaxLength(255)]
    public string TenantId { get; private set; } = string.Empty;
    
    protected void SetTenantId(string tenantId)
    {
        if (string.IsNullOrEmpty(TenantId))
        {
            TenantId = tenantId.ToUpper();
        }
        else
        {
            throw new InvalidOperationException("TenantId is immutable");
        }
    }
}

// For global entities (Claims, Application-wide Roles, etc.)
public abstract class GlobalEntity : AggregateRoot
{
    // No TenantId property
}

// For entities that can be EITHER tenant OR global
public abstract class ScopedEntity : AggregateRoot
{
    [MaxLength(255)]
    public string? TenantId { get; private set; }
    
    public bool IsTenantScoped => !string.IsNullOrEmpty(TenantId);
    public bool IsGlobal => string.IsNullOrEmpty(TenantId);
}
```

**Refactoring required:**
- **User** → stays `TenantEntity` (always tenant-scoped) ✅
- **Role** → becomes `ScopedEntity` (can be tenant OR global) ✅
- **ApiKey** → becomes `ScopedEntity` (can be tenant OR global) ✅
- **Claim** → becomes `GlobalEntity` (always global) ✅
- **Post, Blog, Plant** → stay `TenantEntity` ✅

**Query filter logic:**
```csharp
// TenantEntity: always filter
modelBuilder.Entity<TenantEntity>()
    .HasQueryFilter(e => e.TenantId == currentTenant);

// ScopedEntity: filter OR global
modelBuilder.Entity<ScopedEntity>()
    .HasQueryFilter(e => e.TenantId == currentTenant || e.TenantId == null);

// GlobalEntity: no filter
```

**Tradeoffs:**
- ✅ **Type safety** — compiler enforces tenant vs global
- ✅ **Clear intent** — explicit which entities are multi-tenant
- ✅ **Flexible** — ScopedEntity handles hybrid cases (Role, ApiKey)
- ✅ **Correct query filters** — automatic and safe
- ❌ **Breaking change** — entities must be refactored
- ❌ **Migration required** — schema changes (TenantId NULL/NOT NULL)
- ❌ **More base classes** — increased complexity

**Effort:** 3-4 days (design + implementation + migration + testing)

---

## D. Jarvis's Recommendation

### Recommendation: **Option 4 (Two Base Classes) + Option 1 (Query Filter Extension)**

**Why:**

1. **Option 4 solves the architectural problem correctly:**
   - Clear separation of tenant-scoped vs global entities
   - Type system enforces tenant isolation
   - Removes ambiguity about nullable TenantId
   - Handles hybrid cases (Role, ApiKey) explicitly with ScopedEntity

2. **Option 1 provides the enforcement mechanism:**
   - Automatic query filtering once configured
   - Consumer-friendly API
   - No manual `.Where()` clauses needed

3. **Together, they provide defense-in-depth:**
   - **Design-time:** Type system prevents misuse
   - **Runtime:** Query filters prevent leakage
   - **Write-side:** SaveChanges override (optional) prevents cross-tenant writes

### Implementation Plan

**Phase 1: Create new base classes (non-breaking, additive)**
```csharp
// Add to rbkApiModules.Commons.Core/Abstractions/
public abstract class GlobalEntity : AggregateRoot { }
public abstract class ScopedEntity : AggregateRoot 
{
    [MaxLength(255)]
    public string? TenantId { get; private set; }
    public bool IsTenantScoped => !string.IsNullOrEmpty(TenantId);
    public bool IsGlobal => string.IsNullOrEmpty(TenantId);
}
```

**Phase 2: Create query filter extension**
```csharp
// Add to rbkApiModules.Commons.Relational/
public static void ApplyRbkTenantQueryFilters(
    this ModelBuilder modelBuilder, 
    IHttpContextAccessor httpContextAccessor)
{
    string? GetCurrentTenant()
    {
        return httpContextAccessor.HttpContext?.User
            .FindFirst("tenant")?.Value?.ToUpper();
    }

    // Filter TenantEntity
    foreach (var entityType in modelBuilder.Model.GetEntityTypes()
        .Where(t => typeof(TenantEntity).IsAssignableFrom(t.ClrType)))
    {
        var method = typeof(TenantQueryFilterExtensions)
            .GetMethod(nameof(SetTenantFilter), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(entityType.ClrType);
        method.Invoke(null, new object[] { modelBuilder, GetCurrentTenant });
    }

    // Filter ScopedEntity (tenant OR global)
    foreach (var entityType in modelBuilder.Model.GetEntityTypes()
        .Where(t => typeof(ScopedEntity).IsAssignableFrom(t.ClrType)))
    {
        var method = typeof(TenantQueryFilterExtensions)
            .GetMethod(nameof(SetScopedFilter), BindingFlags.NonPublic | BindingFlags.Static)
            .MakeGenericMethod(entityType.ClrType);
        method.Invoke(null, new object[] { modelBuilder, GetCurrentTenant });
    }
}

private static void SetTenantFilter<T>(
    ModelBuilder modelBuilder, 
    Func<string?> tenantProvider) 
    where T : TenantEntity
{
    modelBuilder.Entity<T>().HasQueryFilter(e => 
        e.TenantId == tenantProvider());
}

private static void SetScopedFilter<T>(
    ModelBuilder modelBuilder, 
    Func<string?> tenantProvider) 
    where T : ScopedEntity
{
    modelBuilder.Entity<T>().HasQueryFilter(e => 
        e.TenantId == tenantProvider() || e.TenantId == null);
}
```

**Phase 3: Mark TenantEntity as [Obsolete] (breaking change warning)**
```csharp
[Obsolete("Use TenantEntity for tenant-only, GlobalEntity for global, or ScopedEntity for hybrid entities")]
public abstract class TenantEntity : AggregateRoot
{
    // Keep existing implementation for backward compatibility
}
```

**Phase 4: Migrate Identity.Core entities (next major version)**
- User → already TenantEntity, keep as-is ✅
- Role → change to ScopedEntity (breaking)
- ApiKey → refactor to inherit ScopedEntity (breaking)
- Claim → change to GlobalEntity (breaking)

**Phase 5: Update SetupTenants() to handle all three types**
```csharp
public static void SetupTenants(this ModelBuilder modelBuilder)
{
    // Setup FK for TenantEntity (required FK)
    // Setup FK for ScopedEntity (optional FK, nullable)
    // Skip GlobalEntity (no FK)
}
```

**Phase 6: Documentation + migration guide**
- README update with tenant isolation setup
- Migration guide for existing consumers
- Breaking change announcement for v2.0

### Why NOT the other options alone?

- **Option 1 alone:** Doesn't fix the architectural confusion about nullable TenantId
- **Option 2 alone:** Only protects writes, not reads
- **Option 3 alone:** Too breaking, doesn't handle hybrid entities like Role

### Is a Breaking Change Acceptable?

**Yes, with proper deprecation path:**

Rodrigo is practical. If we:
1. Mark old approach as `[Obsolete]` in v1.x
2. Provide clear migration guide
3. Ship new base classes as additive in v1.x
4. Make the full breaking change in v2.0

Then **this is the RIGHT fix** vs. a quick patch.

---

## E. Questions for Rodrigo

### Technical Decisions

1. **Hybrid entities (Role, ApiKey):**  
   Role can be tenant-scoped OR application-wide today. Is this intentional design or technical debt? Should we keep this flexibility with `ScopedEntity`, or force all roles to be tenant-scoped?

2. **ApiKey inconsistency:**  
   ApiKey doesn't inherit TenantEntity but has its own TenantId property. Should we refactor it to use the base class hierarchy for consistency?

3. **Breaking change timeline:**  
   Are you comfortable with:
   - v1.x: Add new base classes (`GlobalEntity`, `ScopedEntity`) as additive, mark `TenantEntity` nullable design as obsolete
   - v2.0: Migrate all Identity.Core entities to new hierarchy (breaking)
   
   Or should we do it all at once in v2.0?

4. **Query filter opt-in vs. automatic:**  
   Should `ApplyRbkTenantQueryFilters()` be:
   - **Opt-in** — consumers call it explicitly in `OnModelCreating`
   - **Automatic** — SetupTenants() applies filters automatically
   
   I recommend **opt-in** for control, but automatic is safer.

### Scope Questions

5. **Existing deployments:**  
   Are there production systems using rbkApiModules today? If so, do we need a migration script for existing databases to handle the TenantId nullability change?

6. **Testing strategy:**  
   Should we add integration tests to verify tenant isolation? E.g., "User from tenant A cannot query tenant B's data"?

7. **Priority:**  
   This is marked URGENT in decisions.md. What's the target timeline?
   - Quick fix: Option 1 only (1 day)
   - Proper fix: Option 1 + 4 (4-5 days)

### Verification

8. **Original SetupTenant bug:**  
   Do you remember what the specific error was when TenantId was non-nullable? Stack trace? Migration failure? This would help confirm the root cause.

---

## Summary

**The multi-tenant system is architecturally incomplete:**
- ✅ Entities CAN be tenant-scoped (TenantEntity base class exists)
- ✅ FK relationships are configured (SetupTenants extension)
- ❌ Query filters do NOT exist (manual `.Where()` required everywhere)
- ❌ Nullable TenantId creates confusion (tenant vs global entities)
- ❌ No compile-time enforcement of tenant isolation

**Recommended fix:**
- Create three base classes: `TenantEntity` (required TenantId), `GlobalEntity` (no TenantId), `ScopedEntity` (optional TenantId)
- Provide `ApplyRbkTenantQueryFilters()` extension for automatic query filtering
- Migrate Identity.Core entities in v2.0 with clear deprecation path

**Estimated effort:** 4-5 days (design + implementation + testing + documentation)

**Security impact if not fixed:** HIGH — one forgotten `.Where()` clause leaks cross-tenant data.
