# Wong — History

## Project Context
- **Project:** rbk-api-modules
- **Description:** A comprehensive set of .NET 10 libraries for accelerating ASP.NET Core Web API development. Provides core infrastructure, JWT/API key authentication, EF Core integration, FluentValidation, testing utilities, and static code analyzers.
- **Stack:** C# / .NET 10 / ASP.NET Core / Entity Framework Core / FluentValidation
- **Key documentation:** README.md (root), README-ApiKeys.md, docs/README.md, docs/Commons.Core.md, docs/Identity.Core.md, docs/Identity.Relational.md, docs/Testing.md, docs/Analyzers.md
- **User:** Rodrigo Basniak
- **Team hired:** 2026-04-16

## Learnings

### 2025-01-17: Comprehensive Documentation Audit and Rewrite

**Scope:** Complete documentation overhaul for all consumer-facing features.

**Discoveries:**

1. **Package Structure:**
   - `rbkApiModules.Commons.Core` - Core infrastructure (entities, validation, messaging, file storage, UI definitions, application options)
   - `rbkApiModules.Identity.Core` - Authentication & authorization (JWT, API keys, Windows Auth)
   - `rbkApiModules.Identity.Relational` - Not a separate consumer package; part of Identity.Core
   - `rbkApiModules.Commons.Testing` - Testing framework
   - `rbkApiModules.Analysers` - Code analyzers
   - `rbkApiModules.Messaging.Core` - Internal/domain events (appears to be infrastructure, not well documented for consumers yet)

2. **Major Consumer-Facing Features Found:**
   - JWT authentication with refresh tokens
   - API key authentication with rate limiting (already well-documented in README-ApiKeys.md)
   - Windows Authentication / NTLM support
   - Multi-tenancy with automatic tenant filtering
   - SmartValidator - auto-applies database constraints to FluentValidation rules
   - Dispatcher (CQRS/messaging pattern)
   - UI Definitions system - dynamic form generation from entity metadata
   - Application Options - runtime-editable configuration stored in database
   - Database seeding with deferred seeds
   - Built-in identity management endpoints (users, roles, claims, tenants)
   - Endpoint protection extensions (RequireAuthorizationClaim, RequireAuthenticationApiKey)
   - Testing framework with RbkTestingServer
   - Code analyzers (RBK101-RBK105 for Swagger, RBK201 for authorization)

3. **Extension Methods (Public API):**
   - `AddRbkApiCoreSetup()` - Register core services
   - `AddRbkRelationalAuthentication()` - Register authentication services
   - `AddRbkUIDefinitions()` - Register UI definition system
   - `UseRbkApiCoreSetup()` - Apply core middleware
   - `UseRbkRelationalAuthentication()` - Apply auth middleware
   - `UseRbkUIDefinitions()` - Apply UI definition endpoints
   - `SetupDatabase<T>()` - Run migrations and seeding
   - `SetupRbkDefaultAdmin()` - Create default admin user
   - `SetupRbkAuthenticationClaims()` - Seed built-in claims
   - `SeedDatabase<T>()` - Run custom database seeds
   - `RequireAuthorizationClaim()` - Endpoint filter for claim-based auth
   - `RequireAuthenticationApiKey()` - Endpoint filter for API key-only auth

4. **Base Entities:**
   - `BaseEntity` - Guid ID
   - `TenantEntity` - BaseEntity + TenantId with automatic filtering
   - `AggregateRoot` - Domain event support (for DDD)
   - `EntityReference<T>` - Type-safe reference records

5. **Built-in Identity Endpoints:**
   - Authentication: login (credentials/Windows), register, renew token, change password, reset password, confirm email, switch tenant
   - Users: list, create, activate, deactivate, delete, assign roles, add claim overrides
   - Roles: list, create, rename, delete, update claims
   - Claims: list, create, update, delete, protect, unprotect
   - Tenants: list, create, update, delete
   - API Keys: list, create, update, revoke

6. **Configuration Patterns:**
   - Fluent configuration with `.UseDefault*()` or `.UseCustom*()` methods
   - Option classes: RbkApiCoreOptions, RbkAuthenticationOptions, RbkDefaultAdminOptions
   - All services registered in DI with scoped/singleton lifetimes

7. **Multi-Tenancy Implementation:**
   - Tenant filtering at EF Core query filter level
   - `TenantId` on entities
   - JWT tokens carry tenant claim
   - Tenant switching via dedicated endpoint

**Documents Created:**
- `docs/getting-started.md` - 10-minute quick start guide
- `docs/identity-authentication.md` - JWT, Windows Auth, API keys, multi-tenancy setup
- `docs/identity-management.md` - Users, roles, claims, tenants management

**Documents Updated:**
- `README.md` - Rewritten as concise overview with links (removed lengthy examples)
- `docs/Commons.Core.md` - Updated configuration section (in progress)

**Documents Preserved:**
- `README-ApiKeys.md` - Already excellent, comprehensive API key documentation
- `docs/Testing.md` - Solid testing framework documentation
- `docs/Analyzers.md` - Good analyzer documentation

**Gaps Identified:**

1. **Messaging.Core package** - Appears to be domain/integration events infrastructure but no consumer-facing docs exist. Needs investigation to determine if it's internal-only or requires consumer documentation.

2. **UI Definitions** - Found `AddRbkUIDefinitions()` and `/api/ui-definitions` endpoint but no consumer documentation on how to use it. Feature appears to generate dynamic form metadata from entities.

3. **Application Options** - Found `AddApplicationOptions<T>()`, `AddApplicationOptionsManager()`, and `UseApplicationOptions()` but no docs. Appears to be runtime-editable config stored in database.

4. **Database Seeding** - Found `SetupDatabase()`, `SeedDatabase<T>()`, `AddDeferredSeedRunner()` but no comprehensive guide on how to write seeds or use deferred seeds.

5. **Endpoint definitions** - Pattern using `IEndpoint` interface for mapping endpoints (seen in Demo projects) but not documented.

6. **Email handler** - Found in Commons.Core but only brief mention, no usage guide.

7. **File storage** - IFileStorage interface exists but no setup/usage guide.

8. **Localization** - LocalizationCache and ILocalizationService exist but undocumented.

9. **Built-in endpoints full reference** - Need a comprehensive API reference for all the built-in identity endpoints with request/response schemas.

**Next Steps for Complete Documentation:**
1. Create `docs/ui-definitions.md` - Document UI definition system usage
2. Create `docs/application-options.md` - Document runtime configuration feature
3. Create `docs/seeding.md` - Guide on database seeding and migrations
4. Create `docs/built-in-endpoints.md` - Complete API reference for identity endpoints
5. Update `docs/Commons.Core.md` - Complete rewrite with all features
6. Update `docs/README.md` - Update documentation index with new docs
7. Investigate Messaging.Core - Determine if consumer-relevant, document if so
