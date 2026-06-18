# rbkApiModules — Change History

All notable changes to rbkApiModules are documented here. Changes are organized by release, newest first. Consumer-facing changes only — internal implementation details are excluded.

## X.X.X

### rbkApiModules.Commons.Testing

#### New Features
- **`AspireTestingOptions`** — Project-level configuration for Aspire E2E tests (resource names, login path, localStorage key, API redirect origin, frontend URL).
- **`RbkAspireTestingServer<TAppHost>`** — Frontend URL is now resolved from Aspire (same as backend); optional `FrontendUrlOverride` for local debug when the frontend runs outside the AppHost.

#### Breaking Changes
- ⚠️ **E2E app config moved to a fixture subclass** — Hardcoded defaults (`E2E_FRONTEND_URL`, `gcab_access_token`, `https://localhost:44301`, etc.) were removed. Create a project-specific subclass and override `Options`:

```csharp
public class MyApp_AspireTestingServer : RbkAspireTestingServer<MyApp_AppHost>
{
    protected override AspireTestingOptions Options => new()
    {
        BackendResourceName = "api",
        FrontendResourceName = "webfrontend",
        LoginPath = "/api/ca/login",
        AccessTokenStorageKey = "myapp_access_token",
        ApiRedirectOrigin = "https://localhost:44301",
        FrontendBasePath = "/myapp",
    };
}
```

Use it in tests: `[ClassDataSource<MyApp_AspireTestingServer>(Shared = SharedType.PerClass)]`.

#### Upgrade guide (Aspire E2E / Playwright)

1. **Create a fixture subclass** (see above) and reference it in `[ClassDataSource<...>]` instead of `RbkAspireTestingServer<TAppHost>` directly.
2. **Move `LoginPath`** from `RbkPlaywrightTestBase` (or a local test base override) into `AspireTestingOptions.LoginPath`.
3. **Remove `TestSettings.LoginPath`** — it is no longer used.
4. **Replace `E2E_FRONTEND_URL`** — omit it; the fixture reads the frontend from Aspire. For a frontend running outside Aspire, set `FrontendUrlOverride` in `Options`.
5. **Update `CreateAuthenticatedContextAsync` calls** — signature is now `(username, tenant)` only; login path comes from `Options`.

See [Testing.md — Aspire E2E Testing](Testing.md#aspire-e2e-testing-playwright) for full details.

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



