# rbkApiModules — Change History

All notable changes to rbkApiModules are documented here. Changes are organized by release, newest first. Consumer-facing changes only — internal implementation details are excluded.

## X.X.X

- Package updates across the board

## 10.4.3

### rbkApiModules.Commons.Testing

- Added `FileEncodingChecker` utility for validating file encodings (e.g. UTF-8) in tests.

## 10.4.2

E2E testing bug fixes

## 10.4.1

E2E testing bug fixes

## 10.4.0

### rbkApiModules.Commons.Testing

#### New Features
- **`AspireTestingOptions`** — Project-level configuration for Aspire E2E tests (resource names, frontend port, login path, localStorage key).
- **`RbkAspireTestingServer<TAppHost>`** — Resolves backend URL and API redirect origin from the Aspire `https` endpoint; builds frontend URL as `http://localhost:{FrontendPort}`.

#### Breaking Changes
- ⚠️ **E2E app config moved to a fixture subclass** — Create a project-specific subclass and override `Options`:

```csharp
public class MyApp_AspireTestingServer : RbkAspireTestingServer<MyApp_AppHost>
{
    protected override AspireTestingOptions Options => new()
    {
        BackendResourceName = "backend",
        FrontendResourceName = "frontend",
        FrontendPort = 4207,
        LoginPath = "/api/ca/login",
        AccessTokenStorageKey = "myapp_access_token",
        FrontendBasePath = "/myapp",
    };
}
```

Removed options: `BackendEndpointName` (always `"https"`), `FrontendEndpointName`, `FrontendUrlOverride`, `ApiRedirectOrigin` (derived from Aspire via `GetEndpoint`).

Use it in tests: `[ClassDataSource<MyApp_AspireTestingServer>(Shared = SharedType.PerClass)]`.

#### Upgrade guide (Aspire E2E / Playwright)

1. **Create a fixture subclass** (see above) and reference it in `[ClassDataSource<...>]`.
2. **Move `LoginPath`** into `AspireTestingOptions.LoginPath`.
3. **Remove `TestSettings.LoginPath`** — no longer used.
4. **Replace `FrontendUrlOverride` / `E2E_FRONTEND_URL`** with `FrontendPort` (e.g. `4207`).
5. **Remove `ApiRedirectOrigin`** — derived from the backend's `.WithHttpsEndpoint(..., name: "https")`.
6. **Remove `BackendEndpointName`** — backend must use an endpoint named `"https"`.
7. **Update `CreateAuthenticatedContextAsync` calls** — signature is `(username, tenant)` only.

See [Testing.md — Aspire E2E Testing](Testing.md#aspire-e2e-testing-playwright) for full details.

## 10.3.1

- Fixed a bug in which `QueryResponse.Failure('..')` was not setting the correct status code.

## 10.3.0

#### Breaking Changes
- ⚠️ Added database dispose on `RbkTestingServer<T>.DisposeAsync()`, so if your test methods are deleting the database in the `Cleanup` method, you can remove that call and call `await TestingServer.DisposeAsync()` instead.

## 10.2.0

### rbkApiModules.Identity.Core

#### New Features
- **`ITenantProvider` interface** — Public interface for accessing the current tenant ID from HTTP context. Auto-registered by `AddRbkAuthentication()`. Inject in your services to avoid coupling to `IHttpContextAccessor` and claim names.
- **`ApplyRbkTenantQueryFilters()` extension** — Configure opt-in global query filters in `OnModelCreating()`. Accepts a tenant ID provider and fluent builder to specify per-entity filter modes: `FilterByTenantOnly<T>()` (strict tenant scope), `FilterByTenantOrGlobal<T>()` (tenant + app-wide rows), or `NoFilter<T>()` (no filtering). Eliminates repetitive `.Where(x => x.TenantId == currentTenant)` on every query.

#### Breaking Changes
- ⚠️ **`ApiKey` base class changed** — Now inherits `TenantEntity` instead of `BaseEntity`. Gains a `TenantId` property (nullable `string?`) and participates in tenant isolation. Consumers with custom `ApiKeyConfig` EF configurations that explicitly map `TenantId` property must remove those mappings (now inherited).
- ⚠️ **`AddRbkAuthentication()` and `AddRbkRelationalAuthentication()` signature changed** — Now require an explicit `IConfiguration configuration` as their first parameter. Pass `builder.Configuration` when registering these services. Previously, these methods attempted to resolve configuration from the service container; this removal of the `ServiceProvider.BuildServiceProvider()` anti-pattern ensures compatibility with test contexts like `WebApplicationFactory`.

#### Bug Fixes
- Fixed `SetupTenants()` FK constraint: Now `.IsRequired(false)`, allowing nullable `TenantId` for dual-purpose entities (`ApiKey`, `Role`) that can be both tenant-specific and global.
- Fixed tenant query filter logic: When current tenant is `null` (global admin, test context, unauthenticated), filters now show all rows instead of restricting to `null` TenantId only.
- Fixed `RelationalAuthService` and `RelationalRolesService`: Added `IgnoreQueryFilters()` where appropriate so pre-auth flows (login, refresh token, password reset) and role management aren't blocked by global tenant filters.

### rbkApiModules.Commons.Core

#### Documentation
- Improved `TenantEntity` XML documentation: Clarifies nullable `TenantId` supports hybrid entities (global + tenant-specific).

---

*No prior release history recorded.*







