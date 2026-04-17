# Jarvis — History

## Project Context
- **Project:** rbk-api-modules
- **Description:** A comprehensive set of .NET 10 libraries for accelerating ASP.NET Core Web API development. Provides core infrastructure, JWT/API key authentication, EF Core integration, FluentValidation, testing utilities, and static code analyzers.
- **Stack:** C# / .NET 10 / ASP.NET Core / Entity Framework Core / FluentValidation / xUnit / SQLite (testing) / SQL Server
- **Key packages:** rbkApiModules.Commons.Core, rbkApiModules.Identity.Core, rbkApiModules.Identity.Relational, rbkApiModules.Commons.Testing, rbkApiModules.Analysers
- **Important files:** Directory.Build.props, Directory.Packages.props, global.json, nuget.config
- **User:** Rodrigo Basniak
- **Team hired:** 2026-04-16

## Learnings

### Code Quality Review (2026-04-16)

**Most Common Issues Found:**
1. **Inconsistent null-checking patterns** — Mix of old-style (`== null`) and modern (`is null`) across 50+ instances
2. **TODO/FIXME comments** — 28 instances, including "DONE, REVIEWED" markers that should be deleted
3. **Magic numbers** — 20+ hardcoded timeouts, ports, dimensions scattered throughout (especially in Playwright extensions)
4. **Missing `sealed` keywords** — 10+ classes not designed for inheritance but unsealed
5. **Large methods** — Several 100+ line methods with deep nesting (CoreAuthenticationBuilder, Dispatcher)
6. **Old commented code** — Entire `Old_Dispatcher.cs` file (227 lines) and other commented blocks
7. **Manual null argument checks** — 15+ instances that should use `ArgumentNullException.ThrowIfNull()`
8. **Filename typos** — `ExceptionExtrensions.cs` and `EmumExtensions.cs`

**Packages with Most Debt:**
1. **rbkApiModules.Commons.Core** — Highest complexity, most issues (Dispatcher, SmartValidator, legacy commented code)
2. **rbkApiModules.Identity.Core** — Massive 290-line authentication builder needs refactoring
3. **rbkApiModules.Messaging.Core** — Pervasive "TODO: DONE, REVIEWED" comments to remove
4. **rbkApiModules.Commons.Testing** — Magic numbers in Playwright extensions
5. **rbkApiModules.Analysers** — Cleanest package, minimal issues

**Key Files Needing Attention:**
- `rbkApiModules.Commons.Core\Messaging\Old_Dispatcher.cs` — DELETE (fully commented)
- `rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs` — REFACTOR (290 lines, 8 nesting levels)
- `rbkApiModules.Commons.Core\Messaging\Dispatcher.cs` — REFACTOR (438 lines, extract concerns)
- `rbkApiModules.Commons.Core\Validation\SmartValidator.cs` — MODERNIZE (null checks, simplify)
- `rbkApiModules.Commons.Testing\Playwright\Extensions.cs` — EXTRACT CONSTANTS (magic timeouts)

**Patterns Done Well (to preserve):**
- ✅ **Primary constructors** — Used effectively in newer code (e.g., Dispatcher, handlers)
- ✅ **Record types** — Good use for DTOs and value objects
- ✅ **Pattern matching** — Switch expressions well utilized (ImageUtilities)
- ✅ **Modern async/await** — No `async void` or `.Result`/`.Wait()` blocking found
- ✅ **Dependency injection** — Clean constructor injection throughout, no service locator anti-pattern
- ✅ **Roslyn analyzers** — Clean implementation with minimal technical debt

**Modernization Opportunities:**
- Collection expressions `[]` (C# 12) — Not yet adopted, could simplify array/list initialization
- `required` keyword — Not used, could enforce mandatory properties at compile-time
- `init`-only setters — Limited use, could improve immutability
- `ArgumentNullException.ThrowIfNull()` — Only 2 uses, should be everywhere (C# 11+)

---

### Tenant Query Filter Security Investigation (2026-04-16)

**Investigated:** Multi-tenancy isolation implementation in rbkApiModules.Commons.Core

**Key Findings:**

1. **Class Hierarchy:**
   - `BaseEntity` (Guid Id, no tenant awareness)
   - `AggregateRoot : BaseEntity` (domain events)
   - `TenantEntity : AggregateRoot` (nullable TenantId: string?)

2. **Current Tenant System:**
   - TenantId is **nullable** (`string?`) with TODO comment about non-nullable issues
   - `SetupTenants()` extension creates FK relationships to Tenant table
   - **NO global query filters exist** — developers must manually add `.Where(x => x.TenantId == tenant)` to every query
   - **SECURITY RISK:** One forgotten `.Where()` clause = cross-tenant data leakage

3. **Entity Usage Patterns:**
   - **TenantEntity descendants:** User, Role (hybrid), Post, Blog, Plant
   - **BaseEntity only:** ApiKey (has its own TenantId property!), Claim (global)
   - **No base class:** Tenant (master table)

4. **Why TenantId is Nullable:**
   - Role is designed to be tenant-scoped OR application-wide (`IsApplicationWide` property)
   - Application-wide roles have `TenantId = null`
   - Non-nullable TenantId broke SetupTenants() FK constraints (root cause never fixed)

5. **Architectural Gap:**
   - No distinction between "must be tenant-scoped" vs "can be global" vs "always global"
   - All manual filtering in queries (50+ instances of `.Where(x => x.TenantId == tenant)`)
   - No compile-time enforcement
   - No automatic query filters

**Proposed Solution (Option 4 + 1):**
- Create three base classes:
  - `TenantEntity` (required TenantId) — for User, Post, Blog, Plant
  - `GlobalEntity` (no TenantId) — for Claim
  - `ScopedEntity` (optional TenantId) — for Role, ApiKey (hybrid)
- Add `modelBuilder.ApplyRbkTenantQueryFilters(IHttpContextAccessor)` extension
- Automatic query filtering based on base class type
- Deprecation path: v1.x adds new classes, v2.0 migrates entities

**Alternative Options Considered:**
- Option 1: Query filter extension only (doesn't fix architectural confusion)
- Option 2: SaveChanges override (only protects writes, not reads)
- Option 3: Non-nullable TenantId (too breaking, doesn't handle hybrid entities)

**Impact:** HIGH security concern, URGENT priority per decisions.md

**Effort:** 4-5 days for full solution (design + implementation + migration + testing + docs)

**Next Steps:** Awaiting Rodrigo's decision on:
1. Accept breaking change in v2.0?
2. Keep hybrid entity pattern (ScopedEntity) or force all tenant-scoped?
3. Query filter opt-in vs automatic?
4. Timeline (quick fix vs proper fix)

---

### Tenant Query Filter — Revised Plan (2026-04-16)

**Investigated:** Re-evaluation based on Rodrigo's answers to clarifying questions

**Rodrigo's Constraints:**
1. ✅ Hybrid entities (Role, ApiKey with nullable TenantId) are INTENTIONAL — not a bug
2. ✅ Query filter must be opt-in with per-entity configurability (TenantOnly, TenantOrGlobal, Unfiltered)
3. ✅ Breaking changes in one go — no [Obsolete] markers, no migration guides
4. ❌ Skeptical about GlobalEntity/ScopedEntity base classes — asked for re-evaluation

**Key Insight:**
The current `TenantEntity` with nullable `TenantId` is ALREADY CORRECT. The TODO comment is misleading — it's not about a design flaw, it's about an FK constraint issue when TenantId was non-nullable. The root cause: SetupTenants() creates FK to Tenant table without `.IsRequired(false)`, which breaks when Role or ApiKey use null for global entities.

**Revised Solution (No New Base Classes):**
- ✅ Keep TenantEntity with nullable TenantId (correct as-is)
- ✅ Make ApiKey inherit TenantEntity (consistency + eliminate duplicate TenantId property)
- ✅ Add `modelBuilder.ApplyRbkTenantQueryFilters(tenantProvider, config => {...})` extension
- ✅ Per-entity filter modes: TenantOnly, TenantOrGlobal, Unfiltered
- ✅ Fix SetupTenants() to add `.IsRequired(false)` on FK relationship
- ✅ Replace TODO with clear XML documentation

**Why No New Base Classes:**
- TenantEntity already handles all cases via nullable TenantId
- Entities enforce tenant-scoped vs hybrid in their constructors (User requires tenant, Role accepts null)
- Adding GlobalEntity/ScopedEntity would add complexity without solving a real problem
- The gap is query filtering, not the entity hierarchy

**API Surface:**
```csharp
modelBuilder.ApplyRbkTenantQueryFilters(_tenantProvider, config =>
{
    config.FilterByTenantOnly<User>();        // WHERE TenantId = @current
    config.FilterByTenantOrGlobal<Role>();    // WHERE TenantId = @current OR TenantId IS NULL
    config.FilterByTenantOrGlobal<ApiKey>();  // Same as Role
    config.NoFilter<Claim>();                 // No filter
});
```

**Breaking Changes:**
- ApiKey inheritance change: `BaseEntity` → `TenantEntity` (removes duplicate TenantId property)
- Consumers must add ITenantProvider to DbContext constructor if they want query filters
- Database schema unchanged (ApiKey.TenantId column already exists)

**Effort:** 1 day (implementation + testing)

**Status:** Awaiting Rodrigo's approval of tenant-plan-v2.md

**Open Questions for Rodrigo:**
1. Should ITenantProvider be auto-registered in AddRbkAuthentication() or manual registration?
2. Should Demo projects include query filter examples?

**Recommendation:** APPROVE. This is the minimal correct fix. Query filters solve the security issue, ApiKey consistency fix is clean, no unnecessary architectural changes.

---

### Decision #3 — Remove BuildServiceProvider() Anti-pattern (2026-04-16, revised 2026-04-17)

**What Changed (Revision 2):**  
Previous attempt used `ImplementationInstance` descriptor lookup — broke under `WebApplicationFactory<TProgram>` because `DeferredHostBuilder` registers `IConfiguration` as a factory delegate (`ImplementationFactory`), not as a direct instance. Lookup returned null → `InvalidOperationException` thrown for every test.

**Correct Fix:**  
Added `IConfiguration` as an explicit parameter to both `AddRbkAuthentication` and `AddRbkRelationalAuthentication`. Callers pass `builder.Configuration` at the call site — available immediately from `WebApplicationBuilder`. No descriptor inspection, no container build.

**Signature Changes (breaking):**
```csharp
// Before
AddRbkAuthentication(this IServiceCollection services, RbkAuthenticationOptions options)
AddRbkRelationalAuthentication(this IServiceCollection services, Action<RbkAuthenticationOptions> configureOptions)

// After  
AddRbkAuthentication(this IServiceCollection services, IConfiguration configuration, RbkAuthenticationOptions options)
AddRbkRelationalAuthentication(this IServiceCollection services, IConfiguration configuration, Action<RbkAuthenticationOptions> configureOptions)
```

**Root Cause of First Attempt Failure:**  
`WebApplicationFactory` runs `Program.Main()` via `DeferredHostBuilder.Build()`. When `WebApplicationBuilder` registers `IConfiguration`, it uses `.AddSingleton<IConfiguration>(sp => _config)` (factory pattern), not `.AddSingleton<IConfiguration>(instance)` (instance pattern). My descriptor lookup specifically checked `ImplementationInstance`, which is null for factory registrations.

**Test Setup:**  
Tests use `WebApplicationFactory<TProgram>` (via `RbkTestingServer<T>`) — full app startup, not a bare `ServiceCollection`. The fix needed to work within that startup flow.

**Key Files:**
- `rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs`
- `rbkApiModules.Identity.Core\Relational\Builder.cs`
- `Demo1\Program.cs`
- `Demo2\Program.cs`

**Breaking Change:** YES — public signature changed for `AddRbkAuthentication` and `AddRbkRelationalAuthentication`.  
**Build Status:** ✅ Succeeded (0 errors).  
**Test Status:** ✅ 350/350 passed.

---

### Tenant Query Filter Implementation (2026-04-16)

**Implemented:** Opt-in tenant query filters based on approved tenant-plan-v2.md

**Files Changed:**
1. **ITenantProvider interface** — `rbkApiModules.Identity.Core\Core\Contracts\ITenantProvider.cs`
   - Public interface for tenant resolution (needed for ModelBuilder extension signature)
   - Returns `string? CurrentTenantId` property
   - Re-evaluated on every access (never cached)

2. **HttpContextTenantProvider** — `rbkApiModules.Identity.Core\Core\Services\HttpContextTenantProvider.cs`
   - Internal sealed implementation
   - Extracts tenant from `HttpContext.User` claims using `JwtClaimIdentifiers.Tenant` ("tenant" claim)
   - Uppercases tenant ID for consistency with TenantEntity behavior
   - Returns null if HttpContext or claim not available

3. **RbkTenantFilterBuilder** — `rbkApiModules.Identity.Core\Relational\Utilities\RbkTenantFilterBuilder.cs`
   - Fluent builder for configuring EF Core query filters
   - Three filter modes:
     - `FilterByTenantOnly<T>()` — WHERE TenantId = CurrentTenantId (for User, Post, Blog)
     - `FilterByTenantOrGlobal<T>()` — WHERE TenantId = CurrentTenantId OR TenantId IS NULL (for Role, ApiKey)
     - `NoFilter<T>()` — Explicitly no filter (for Claim, Tenant)
   - Uses Expression trees to capture ITenantProvider reference (not value) for dynamic re-evaluation

4. **ModelBuilderExtensions.ApplyRbkTenantQueryFilters()** — `rbkApiModules.Identity.Core\Relational\Utilities\ModelBuilderExtensions.cs`
   - Public extension method called in DbContext.OnModelCreating
   - Takes ITenantProvider instance (from DI via DbContext constructor)
   - Accepts configuration callback to define per-entity filter modes

5. **ModelBuilderExtensions.SetupTenants()** — Updated
   - Added `.IsRequired(false)` to FK relationship (fixes nullable TenantId constraint issue)
   - Now correctly supports hybrid entities (Role, ApiKey) with null TenantId

6. **ApiKey entity** — `rbkApiModules.Identity.Core\Core\Models\Entities\ApiKey.cs`
   - Changed base class from `BaseEntity` to `TenantEntity`
   - Removed duplicate `TenantId` property (now inherited)
   - Removed `NormalizeTenantId()` method (TenantEntity handles uppercasing)
   - Updated constructor to use TenantId setter from base class

7. **TenantEntity documentation** — `rbkApiModules.Commons.Core\Abstractions\TenantEntity.cs`
   - Removed Portuguese TODO comment
   - Added XML documentation explaining nullable TenantId design (hybrid entities pattern)

8. **CoreAuthenticationBuilder registration** — `rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs`
   - Auto-registers `ITenantProvider` as scoped service in `AddRbkAuthentication()`
   - Ensures `AddHttpContextAccessor()` is called
   - Implementation detail — consumers never manually register ITenantProvider

9. **Demo1 DatabaseContext** — `Demo1\Database\DatabaseContext.cs`
   - Added `ITenantProvider` constructor parameter
   - Added `ApplyRbkTenantQueryFilters()` call with filters for User, Role, Post, Blog, ApiKey

10. **Demo2 DatabaseContext** — `Demo2\Database\DatabaseContext.cs`
    - Added `ITenantProvider` constructor parameter
    - Added `ApplyRbkTenantQueryFilters()` call with filters for User, Role, ApiKey

11. **Demo1 DatabaseContextFactory** — `Demo1\DatabaseContextFactory.cs`
    - Added design-time `ITenantProvider` implementation (returns null)
    - Required for EF Core migrations tooling

**Patterns Used:**
- Claim name for tenant: `JwtClaimIdentifiers.Tenant` = "tenant"
- Namespace conventions: `rbkApiModules.Identity.Core` for core abstractions, `rbkApiModules.Identity` for relational utilities
- ITenantProvider is public (required for public method signature), but implementation is internal sealed
- Expression tree pattern captures provider reference (not value) for EF Core dynamic evaluation

**Build Status:** ✅ Succeeded (0 errors, 446 warnings)

**Commit:** da190b6d — "feat: add opt-in tenant query filters and fix ApiKey base class"

**Breaking Changes:**
- ApiKey now inherits TenantEntity — consumers with custom EF config for ApiKey.TenantId must remove it
- DbContext subclasses requiring query filters must add ITenantProvider to constructor (auto-resolved from DI)

**Security Impact:** Closes tenant isolation gap — filters applied automatically on configured entities, manual `.Where()` clauses no longer required

**Next Steps:** Update consumer projects to add `ApplyRbkTenantQueryFilters()` in their DbContext if they use multi-tenancy

