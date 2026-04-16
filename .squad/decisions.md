# Squad Decisions

## Active Decisions

### Documentation Structure (2026-01-17)
**Decision Maker:** Wong (Documentation Writer)  
**Status:** Implemented

Restructured rbkApiModules documentation from single lengthy README to a hub-and-spoke model:
- **Root README.md** - Concise overview (under 200 lines) with package descriptions and links
- **docs/getting-started.md** - 10-minute quick start guide
- **Specialized docs** - One doc per major feature area

**Structure:**

1. **README.md** (root) - Overview only
   - What is rbkApiModules (2-3 sentences)
   - Package table with one-line descriptions
   - Quick install commands
   - Minimal code sample (10-15 lines)
   - Links to detailed docs

2. **docs/getting-started.md** - Quick start guide
   - Prerequisites, 8-step setup, first login test, first protected endpoint

3. **docs/identity-authentication.md** - Authentication setup
   - JWT, Windows Auth, API keys, multi-tenancy, endpoint protection

4. **docs/identity-management.md** - Identity management
   - Users, roles, claims, tenants CRUD operations

5. **docs/Commons.Core.md** - Core infrastructure
   - Base entities, SmartValidator, Dispatcher, Authentication, File storage, Email

6. **docs/Testing.md** - Testing framework (preserved)
7. **docs/Analyzers.md** - Code analyzers (preserved)
8. **README-ApiKeys.md** - API keys (preserved as-is)

**Rationale:** Hub-and-spoke model improves discoverability vs. monolithic README. Progressive disclosure: overview → quick start → feature-specific depth. Easier to maintain and search.

**Gaps for future documentation:** Messaging.Core, UI Definitions, Application Options, Database seeding, Email handler, File storage, Localization, Complete endpoint API reference, IEndpoint pattern.

---

## Architectural Decisions — Phase 2 Review (2026-04-16)
**Status:** PENDING FURY APPROVAL  
**From:** Stark (Architect), Jarvis (Backend Developer)  
**Review Needed By:** Fury (Tech Lead)

### 1. ~~Create rbkApiModules.Commons.Relational Package~~ — REJECTED
**Severity:** N/A — Architecture is intentional  
**Decision:** REJECTED by Rodrigo Basniak (2026-04-16)

**Directive:**
All consumers of rbkApiModules are assumed to use EF Core. The single-package-per-domain architecture (`Commons.Core`, `Identity.Core`) is a deliberate design decision made when the old Core/Relational split was merged. EF Core code inside `Commons.Core` is correct and expected — not a violation.

There is no `rbkApiModules.Identity.Relational` package. The correct package is `rbkApiModules.Identity.Core`. The old split is gone.

**What to do instead:**  
The namespace strings in some files still reference `.Relational` namespaces (legacy from the old split). These should be corrected to use `.Core` namespaces — but this is a cleanup task, not a package split.

**Status:** ❌ REJECTED — do not revisit

---

### 2. 🔴 Fix Tenant Query Filter Security Bug (URGENT)
**Severity:** HIGH — Security Concern  
**Decision Needed:** Investigation priority and approach

**Issue:**
In `rbkApiModules.Commons.Core\Abstractions\TenantEntity.cs:14`:
```csharp
// TODO: Estava dando problema no SetupTenant quando o tenantId nao era nulavel. 
// Tem que descobrir e corrigir isso
```
Translation: "Was having issues with SetupTenant when tenantId was not nullable. Need to find and fix this."

**Impact:**
- Suggests known bug with multi-tenant isolation
- Potential tenant data leakage if non-nullable TenantId causes query filter bypass
- No global query filter helper provided — consumers must manually apply filters (high risk)

**Proposed Solution:**
1. Investigate the original bug with non-nullable TenantId
2. Provide `modelBuilder.ApplyTenantQueryFilters()` extension
3. Document tenant isolation setup in README
4. Add integration test to verify tenant isolation

**Effort:** 4-8 hours (investigation + fix)  
**Breaking Change:** NO (additive only)  
**Status:** ⏳ AWAITING DECISION (RECOMMEND IMMEDIATE)

---

### 3. 🔴 Remove ServiceProvider.Build() Anti-pattern
**Severity:** HIGH — Correctness Issue  
**Decision Needed:** Approval for refactor

**Issue:**
In `CoreAuthenticationBuilder.cs:26`:
```csharp
var serviceProvider = services.BuildServiceProvider();
var configuration = serviceProvider.GetService<IConfiguration>();
```

This anti-pattern:
- Creates disposed scope risk
- Impacts performance
- Violates ASP.NET Core DI guidelines

**Proposed Solution:**
Refactor to accept `IConfiguration` via options lambda (internal implementation change only)

**Effort:** 2-4 hours  
**Breaking Change:** NO (internal implementation)  
**Status:** ⏳ AWAITING DECISION

---

### 4. 🟠 Dispatcher Architecture Refactoring
**Severity:** HIGH — Code Quality  
**Decision Needed:** Refactor approach

**Issue:**
`rbkApiModules.Commons.Core\Messaging\Dispatcher.cs` is a 438-line god class with a 235-line `SendAsync<TResponse>()` method handling validation, identity propagation, handler resolution, behavior pipeline execution, and telemetry with 5-6 levels of nesting.

**Options:**
- **A. Extract Separate Services** — Clean separation, testable, extensible (but more classes)
- **B. Extract Private Methods** — Simpler, keeps logic together (but still large class)
- **C. Keep As-Is** — No change risk (but maintains technical debt)

**Recommendation:** **Option A** — Extract to separate services for long-term maintainability.

**Decision Required:** Which approach? Timeline for refactor?  
**Status:** ⏳ AWAITING DECISION

---

### 5. 🟠 CoreAuthenticationBuilder Decomposition
**Severity:** HIGH — Code Quality  
**Decision Needed:** Refactor approach

**Issue:**
`rbkApiModules.Identity.Core\Core\CoreAuthenticationBuilder.cs` has a single 290-line `AddRbkAuthentication()` method with 8 levels of nesting, handling JWT config, email setup, API key auth, rate limiting, and authorization. Mixing Spanish and English comments.

**Options:**
- **A. Fluent Builder Pattern** — Clean API, composable, easy to test (significant refactor, breaking change)
- **B. Extract Static Helper Methods** — Simpler refactor, preserves existing API (somewhat monolithic)
- **C. Separate Extension Methods** — Maximum flexibility, clean separation (breaking change)

**Recommendation:** **Option B** short-term, **Option A** long-term for better API design.

**Decision Required:** Which approach? Is a breaking change acceptable?  
**Status:** ⏳ AWAITING DECISION

---

### 6. 🟠 ConfigureAwait Usage Policy
**Severity:** MEDIUM — Standardization  
**Decision Needed:** Official policy

**Issue:**
Only 5 instances of `.ConfigureAwait(false)` in entire codebase. Inconsistent usage. Library code typically should use it to avoid deadlocks in synchronous contexts.

**Options:**
- **A. Add Everywhere in Library Code** — Best practice for libraries (~200+ locations, large change)
- **B. Don't Use** — Rely on .NET Core behavior (simpler, works in ASP.NET Core)
- **C. Use Only in Background Services** — Targeted approach (inconsistent policy)

**Recommendation:** **Option B** with documentation — .NET Core's lack of SynchronizationContext makes ConfigureAwait largely unnecessary in modern ASP.NET Core. Document this decision. Add analyzer enforcement if needed.

**Decision Required:** What's the official policy? Analyzer needed?  
**Status:** ⏳ AWAITING DECISION

---

### 7. 🟠 Multi-Tenancy TenantId Nullability
**Severity:** MEDIUM — Architecture Clarity  
**Decision Needed:** Multi-tenancy strategy

**Issue:**
`TenantEntity.TenantId` is nullable (`string?`) with unresolved TODO. Unclear multi-tenancy strategy. Nullable design allows entities without tenants (intentional?).

**Options:**
- **A. Make TenantId Required** — Clear enforcement, type safety (breaking change, need separate base for global entities)
- **B. Create Two Base Classes** — Clear separation for global vs tenant entities (more base classes, refactor needed)
- **C. Keep Nullable, Document** — Flexible, no breaking change (doesn't solve underlying bug)

**Recommendation:** **Option B** — Clear architecture with separate base classes for global vs tenant entities.

**Decision Required:** Intended multi-tenancy model? Can we make breaking change?  
**Status:** ⏳ AWAITING DECISION

---

### 8. 🟠 Poison Message Handling Strategy
**Severity:** MEDIUM — Production Reliability  
**Decision Needed:** DLQ implementation approach

**Issue:**
`rbkApiModules.Messaging.Core\Events\Messaging\BaseIntegrationConsumer.cs` has TODO: "Poison message strategy: Record last error and implement a max-attempts policy (e.g., move to DLQ or stop retrying after N attempts)."

**Impact:**
- No retry limit on failed integration events
- Failed messages retry indefinitely
- No dead-letter queue (DLQ) for permanently failed messages
- Potential for infinite retry loops

**Options:**
- **A. DLQ with Max Retries** — Industry standard, prevents infinite retries (requires schema changes)
- **B. Simple Max Retries** — Simpler implementation (no visibility into failed messages)
- **C. Exponential Backoff Only** — Eventually might succeed (bad messages never cleared)

**Recommendation:** **Option A** — Implement proper DLQ pattern with configurable max retries (default: 5) and exponential backoff. Add dashboard/endpoint to inspect DLQ.

**Decision Required:** Which strategy? Max retry count? Need DLQ visibility tools?  
**Status:** ⏳ AWAITING DECISION

---

### 9. 🟠 Standardize Public API Naming
**Severity:** MEDIUM — Consistency  
**Decision Needed:** Naming convention

**Issue:**
- Most extensions: `AddRbk*` (e.g., `AddRbkApiCoreSetup`, `AddRbkAuthentication`)
- Messaging: `AddMessaging` ❌ (inconsistent — missing "Rbk" prefix)

**Proposed Solution:**
Rename `AddMessaging` → `AddRbkMessaging` for consistency

**Effort:** 1 hour  
**Breaking Change:** YES (minor — method rename)  
**Status:** ⏳ AWAITING DECISION

---

### 10. 🟢 Remove or Document IEndpoint Pattern
**Severity:** LOW — Code Clarity  
**Decision Needed:** Pattern fate

**Issue:**
In `rbkApiModules.Commons.Core\Abstractions\IEndpoint.cs:5`:
```csharp
// TODO: remove, don't want black magic happening anymore
public interface IEndpoint
{
    static abstract void MapEndpoint(IEndpointRouteBuilder endpoints);
}
```

- Marked for removal but still in codebase
- `EndpointAutoMapper` references it
- Demos use direct endpoint mapping (don't use the pattern)

**Options:**
- **A.** Remove interface + EndpointAutoMapper (demos prove it's not needed)
- **B.** Keep and document as optional pattern (remove TODO)

**Effort:** 30 minutes  
**Breaking Change:** Only if option A and someone uses it  
**Status:** ⏳ AWAITING DECISION

---

### 11. 🟢 Additional Easy Wins (No Decision Required)
**Status:** READY TO IMPLEMENT

- ✅ Delete Old_Dispatcher.cs (227 lines of commented code) — Git preserves it
- ✅ Translate Spanish comments in CoreAuthenticationBuilder to English
- ✅ Standardize null checking pattern across codebase (C# 8+ null-coalescing)
- ✅ Inventory and clean up 28 TODOs with context

---

## Decision Summary Table

| # | Decision | Priority | Breaking? | Blocking? | Effort | Status |
|---|----------|----------|-----------|-----------|--------|--------|
| 1 | ~~Commons.Relational package~~ | N/A | N/A | N/A | N/A | ❌ REJECTED |
| 2 | Tenant query filter bug | HIGH | NO | YES | 4-8h | 🔴 URGENT |
| 3 | ServiceProvider.Build() | HIGH | NO | NO | 2-4h | ⏳ Ready |
| 4 | Dispatcher refactoring | HIGH | NO | NO | TBD | ⏳ Ready |
| 5 | CoreAuthenticationBuilder | HIGH | YES | NO | TBD | ⏳ Ready |
| 6 | ConfigureAwait policy | MEDIUM | NO | NO | Var | ⏳ Ready |
| 7 | TenantId nullability | MEDIUM | YES | NO | TBD | ⏳ Ready |
| 8 | Poison message handling | MEDIUM | NO | NO | 2-3d | ⏳ Ready |
| 9 | API naming | MEDIUM | YES | NO | 1h | ⏳ Ready |
| 10 | IEndpoint pattern | LOW | Maybe | NO | 30m | ⏳ Ready |

---

## Governance

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction

- All meaningful changes require team consensus
- Document architectural decisions here
- Keep history focused on work, decisions focused on direction
