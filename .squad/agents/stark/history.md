# Stark — History

## Project Context
- **Project:** rbk-api-modules
- **Description:** A comprehensive set of .NET 10 libraries for accelerating ASP.NET Core Web API development. Provides core infrastructure, JWT/API key authentication, EF Core integration, FluentValidation, testing utilities, and static code analyzers.
- **Stack:** C# / .NET 10 / ASP.NET Core / Entity Framework Core / FluentValidation / xUnit / SQLite (testing) / SQL Server
- **Key packages:** rbkApiModules.Commons.Core, rbkApiModules.Identity.Core, rbkApiModules.Identity.Relational, rbkApiModules.Commons.Testing, rbkApiModules.Analysers
- **User:** Rodrigo Basniak
- **Team hired:** 2026-04-16

## Learnings

### 2025-04-16: Comprehensive Architecture Analysis

**Engagement:** Full codebase architectural review post-refactoring

**Key Architectural Patterns Found:**
- **Core/Relational Separation**: Identity package properly separates core abstractions from EF Core implementations, but Commons package violates this pattern
- **Dispatcher/CQRS**: Clean implementation with `IDispatcher`, `IRequestHandler<TRequest, TResponse>`, automatic handler registration via reflection
- **Options Pattern**: Fluent builder pattern used consistently (`RbkApiCoreOptions`, `RbkAuthenticationOptions`)
- **Exception Handling**: Global middleware with RFC 7807 ProblemDetails, three-tier exception hierarchy (Expected 400, Unexpected 500, Validation)
- **Multi-tenancy**: Claims-based tenant propagation, `TenantEntity` base class, cross-tenant API keys supported but manual query filtering required
- **Authentication Schemes**: JWT Bearer (default), API Key (custom handler), Windows Auth (middleware → JWT), all write to same ClaimsPrincipal format

**Most Significant Design Issues:**
1. **Missing Package**: `rbkApiModules.Commons.Relational` namespace exists but package doesn't - all EF Core code lives in Commons.Core (dependency graph violation)
2. **Namespace Chaos**: 30+ files with namespace != physical location (`Commons.Core` project contains `namespace Commons.Relational`)
3. **DI Anti-pattern**: `services.BuildServiceProvider()` called during configuration in `CoreAuthenticationBuilder` (creates disposed scope risk)
4. **Tenant Security Bug**: TODO comment indicates known issue with non-nullable TenantId in multi-tenant setup
5. **No Global Query Filter**: Consumers must manually apply tenant filters (high leakage risk)

**Dependency Graph Summary:**
```
Current (BROKEN):
  Commons.Core → EF Core (should not!)
  Identity.Core → Commons.Core (includes EF Core transitively)
  Messaging.Core → Commons.Core (includes EF Core transitively)
  Testing.Core → Commons.Core, Identity.Core

Should Be:
  Commons.Core → (abstractions only)
  Commons.Relational → Commons.Core + EF Core
  Identity.Core → Commons.Core
  Identity.Relational → Identity.Core + Commons.Relational
  Messaging.Core → Commons.Core
```

**Important File Paths:**

*Entrypoints:*
- `rbkApiModules.Commons.Core\Features\CoreSetup\Builder.cs` - `AddRbkApiCoreSetup`, `UseRbkApiCoreSetup`
- `rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs` - `AddRbkAuthentication`
- `rbkApiModules.Identity.Core\Relational\Builder.cs` - `AddRbkRelationalAuthentication`, `UseRbkRelationalAuthentication`
- `rbkApiModules.Commons.Core\Messaging\Builder.cs` - `AddMessaging`, `AddTransactionalPipelineBehavior`

*Key Abstractions:*
- `rbkApiModules.Commons.Core\Messaging\Dispatcher.cs` - CQRS dispatcher
- `rbkApiModules.Commons.Core\Abstractions\TenantEntity.cs` - Multi-tenant base entity (has TODO bug)
- `rbkApiModules.Commons.Core\Authentication\AuthenticatedRequest.cs` - Request context with user/tenant
- `rbkApiModules.Identity.Core\Core\Models\Entities\User.cs` - User entity with role/claim processing
- `rbkApiModules.Commons.Core\Middleware\ExceptionHandlingMiddleware.cs` - Global error handler

*Problem Areas:*
- `rbkApiModules.Commons.Core\Database\**` - Should be in separate Relational package
- `rbkApiModules.Identity.Core\Relational\Config\**` - Wrong namespace (`rbkApiModules.Authentication`)
- `rbkApiModules.Commons.Core\Messaging\Builder.cs:26` - DI anti-pattern (builds ServiceProvider)

**Technical Debt:**
- 12 TODO comments (3 in Portuguese)
- Many "TODO: DONE, REVIEWED" noise comments
- Portuguese XML doc comments in multiple files
- `IEndpoint` marked for removal but still in use

---

### Tenant Query Filter Implementation Validation (2026-04-16)

**Engagement:** Post-implementation architectural validation of tenant query filter system

**Findings:**

✅ **Architecture Validated** — The opt-in tenant query filter implementation closes the identified security gap without introducing unnecessary architectural complexity.

**Key Validations:**

1. **No New Base Classes** — Initial proposal to add GlobalEntity/ScopedEntity was rejected correctly. The existing TenantEntity with nullable TenantId adequately handles all entity patterns:
   - TenantOnly entities (User, Post, Blog) — all queries filtered to current tenant
   - Hybrid entities (Role, ApiKey) — queries filtered to current tenant OR global (null)
   - Global entities (Claim, Tenant) — no filtering
   - Per-entity configurability provides the control without new hierarchy

2. **SetupTenants() Fix** — FK constraint now correctly marked as `.IsRequired(false)`, enabling hybrid entities to use null for global scope. This was the root cause of the original TODO.

3. **ITenantProvider Design** — Correct decision to auto-register and keep internal:
   - Public interface (required for public API)
   - Internal sealed implementation (no consumer extensibility)
   - Scoped lifetime (re-evaluated per request)
   - Auto-registration (boilerplate reduction)

4. **Query Filter Expression Tree Pattern** — Well-designed for EF Core:
   - Captures provider reference (not value) for dynamic re-evaluation
   - Supports multi-tenancy without query-time tenant parameter passing
   - Clean separation between entity configuration and tenant resolution

5. **Opt-In Design** — Correct architectural choice:
   - Consumers explicitly enable filters per entity
   - Per-entity filter modes provide flexibility (TenantOnly vs TenantOrGlobal)
   - No automatic filtering surprises
   - Parallel support for both filtered and unfiltered queries (via NoFilter mode)

**Security Assessment:**
- ✅ Closes cross-tenant data leakage risk (automatic vs manual `.Where()`)
- ✅ No bypasses available (once configured, filtering is enforced)
- ✅ Clear API prevents misuse (explicit configuration required)

**Design Debt Resolution:**
- ✅ TenantEntity TODO comment replaced with clear documentation
- ✅ ApiKey base class inconsistency fixed (now inherits TenantEntity)
- ✅ SetupTenants() constraint issue resolved

**Breaking Changes Assessment:**
- ✅ Minimal blast radius (ApiKey EF config, DbContext constructor, EF migrations)
- ✅ One-go approach (no phased migration needed)
- ✅ Clear migration path in documentation

**Next Phase:** Documentation and consumer migration guide (tasked to Wong)
