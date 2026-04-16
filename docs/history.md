# rbkApiModules — Change History

All notable changes to rbkApiModules are documented here. Changes are organized by release, newest first. Consumer-facing changes only — internal implementation details are excluded.

## X.X.X

### rbkApiModules.Identity.Core

#### New Features
- **`ITenantProvider` interface** — Public interface for accessing the current tenant ID from HTTP context. Auto-registered by `AddRbkAuthentication()`. Inject in your services to avoid coupling to `IHttpContextAccessor` and claim names.
- **`ApplyRbkTenantQueryFilters()` extension** — Configure opt-in global query filters in `OnModelCreating()`. Accepts a tenant ID provider and fluent builder to specify per-entity filter modes: `FilterByTenantOnly<T>()` (strict tenant scope), `FilterByTenantOrGlobal<T>()` (tenant + app-wide rows), or `NoFilter<T>()` (no filtering). Eliminates repetitive `.Where(x => x.TenantId == currentTenant)` on every query.

#### Breaking Changes
- ⚠️ **`ApiKey` base class changed** — Now inherits `TenantEntity` instead of `BaseEntity`. Gains a `TenantId` property (nullable `string?`) and participates in tenant isolation. Consumers with custom `ApiKeyConfig` EF configurations that explicitly map `TenantId` property must remove those mappings (now inherited).

#### Bug Fixes
- Fixed `SetupTenants()` FK constraint: Now `.IsRequired(false)`, allowing nullable `TenantId` for dual-purpose entities (`ApiKey`, `Role`) that can be both tenant-specific and global.
- Fixed tenant query filter logic: When current tenant is `null` (global admin, test context, unauthenticated), filters now show all rows instead of restricting to `null` TenantId only.
- Fixed `RelationalAuthService` and `RelationalRolesService`: Added `IgnoreQueryFilters()` where appropriate so pre-auth flows (login, refresh token, password reset) and role management aren't blocked by global tenant filters.

### rbkApiModules.Commons.Core

#### Documentation
- Improved `TenantEntity` XML documentation: Clarifies nullable `TenantId` supports hybrid entities (global + tenant-specific).

---

*No prior release history recorded.*
