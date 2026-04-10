# API Keys

This document describes the API key authentication system in `rbkApiModules.Identity.Core`. It covers setup, claims, authentication, rate limiting, management endpoints, and how to protect your own endpoints.

---

## Table of Contents

1. [Overview](#overview)
2. [Setup](#setup)
3. [Key Format](#key-format)
4. [Claims & Permissions](#claims--permissions)
5. [Tenant Scoping](#tenant-scoping)
6. [Making Authenticated Requests](#making-authenticated-requests)
7. [Protecting Your Endpoints](#protecting-your-endpoints)
8. [Rate Limiting](#rate-limiting)
9. [Management Endpoints](#management-endpoints)
10. [Caching](#caching)
11. [Usage Tracking](#usage-tracking)
12. [Seeding API Keys](#seeding-api-keys)

---

## Overview

API keys provide machine-to-machine authentication without JWT tokens. Each key:

- Is stored only as a SHA-256 hash — the raw value is shown **once** at creation time and never retrievable again
- Carries a set of application claims that control access, exactly like JWT roles
- Has an optional expiration date and can be revoked permanently
- Is optionally scoped to a specific tenant
- Is subject to per-key token-bucket rate limiting

---

## Setup

Call `AddApiKeyAuthentication` inside `AddRbkRelationalAuthentication`:

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .UseLoginWithWindowsAuthentication()
    .AddApiKeyAuthentication()
);
```

Apply the middleware by calling `UseRbkRelationalAuthentication`, which handles the correct middleware ordering automatically:

```csharp
app.UseRbkRelationalAuthentication();
```

Internally, `AddApiKeyAuthentication` will:

- Register the `Api-Key` authentication scheme
- Add `IMemoryCache` for auth-result and rate-limit caching
- Register `IApiKeyUsageTracker` for daily request counters
- Register `IApiKeyLastUsedThrottler` for throttled `LastUsedAt` writes
- Configure ASP.NET Core's `RateLimiter` with a per-key token-bucket policy
- Extend the default authorization policy to accept both JWT and API key callers

Also ensure the built-in seeding includes descriptions for the two new claims that ship with API key support:

```csharp
app.SetupRbkAuthenticationClaims(options => options
    // ... other claim descriptions ...
    .WithCustomDescription(x => x.ManageApiKeys, "Manage API keys")
    .WithCustomDescription(x => x.ManageCrossTenantApiKeys, "Create and manage cross-tenant API keys")
);
```

### Configuration options

`AddApiKeyAuthentication` accepts an optional lambda to tune the defaults:

```csharp
.AddApiKeyAuthentication(options =>
{
    // How long an auth result is held in IMemoryCache before a database re-check (default: 10 minutes)
    options.CacheAbsoluteExpiration = TimeSpan.FromMinutes(10);

    // Default RequestsPerMinute used when the client omits the value at creation time,
    // and also the fallback rate applied to unknown/expired/revoked keys (default: 600)
    options.RequestsPerMinute = 600;

    // Minimum interval between LastUsedAt writes to the database (default: 2 minutes)
    options.LastUsedUpdateMinInterval = TimeSpan.FromMinutes(2);
})
```

---

## Key Format

Generated keys follow the pattern:

```
rbk_live_<64-hex-chars>
```

Example:

```
rbk_live_fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210
```

- The prefix `rbk_live_` identifies the scheme and is stored in `KeyPrefix`
- The secret segment is 32 cryptographically random bytes encoded as lowercase hex
- The raw key is **never stored** — only its SHA-256 hash is persisted in `KeyHash`
- `KeyPrefix` is stored separately so you can identify a key in the database without revealing the secret

---

## Claims & Permissions

### Management claims (JWT callers only)

These claims control who can call the management endpoints. They must be assigned through the normal role/claim system and only take effect for JWT-authenticated callers.

| Claim identification              | Purpose                                                                              |
|-----------------------------------|--------------------------------------------------------------------------------------|
| `CAN_MANAGE_APIKEYS`              | Required to list, create, update, and revoke API keys                                |
| `CAN_MANAGE_CROSS_TENANT_API_KEYS`| Required to create global (tenant-less) keys or manage keys belonging to other tenants |

> All management endpoints are restricted to JWT callers. An API key holder, even one carrying `CAN_MANAGE_APIKEYS`, cannot call these endpoints.

### Claims assignable to API keys

Not every claim can be placed on an API key. Each claim has an `AllowApiKeyUsage` flag that must be explicitly enabled:

**When creating a claim:**
```json
POST /api/authorization/claims
{
  "identification": "MY_INTEGRATION",
  "description": "Allow integration access",
  "allowApiKeyUsage": true
}
```

**When updating a claim:**
```json
PUT /api/authorization/claims
{
  "id": "<guid>",
  "description": "Allow integration access",
  "allowApiKeyUsage": true
}
```

API key creation and update will fail with a validation error if any of the assigned claims do not have `AllowApiKeyUsage = true`.

When a request is authenticated via API key the assigned claims populate the `roles` claim in the `ClaimsPrincipal`, making them interchangeable with JWT role claims from the authorization perspective.

---

## Tenant Scoping

An API key is either **global** (no tenant) or scoped to a **specific tenant**.

| Key `TenantId` | Who can create it | Who can see and manage it |
|---|---|---|
| `null` (global) | Caller with `CAN_MANAGE_CROSS_TENANT_API_KEYS` | Callers with `CAN_MANAGE_CROSS_TENANT_API_KEYS` |
| `"ACME"` | Caller whose JWT tenant is `ACME`, or a caller with `CAN_MANAGE_CROSS_TENANT_API_KEYS` | Callers whose JWT tenant is `ACME`, or callers with `CAN_MANAGE_CROSS_TENANT_API_KEYS` |

When a tenant-scoped key is used for authentication, the resulting `ClaimsPrincipal` carries the tenant ID that is stored on the key.

---

## Making Authenticated Requests

Send the raw key in the `Api-Key` request header:

```http
GET /your/endpoint HTTP/1.1
Host: api.example.com
Api-Key: rbk_live_fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210
```

Authentication will fail (returning `401 Unauthorized`) if:

- The key does not start with `rbk_live_`
- The SHA-256 hash does not match any record in the database
- The matching key has `IsActive = false` or has been revoked
- The key has passed its `ExpirationDate`
- Any claim assigned to the key does not have `AllowApiKeyUsage = true`

A successful authentication produces the following claims on the `ClaimsPrincipal`:

| Claim type | Value |
|---|---|
| `ClaimTypes.Name` | Key name |
| `roles` | One entry per assigned claim identification |
| `tenant` | `TenantId` of the key (empty string for global keys) |
| `displayName` | Key name |
| `authMode` | `ApiKey` |
| `rbk-apikey-id` | GUID of the key entity |

---

## Protecting Your Endpoints

### API key only

Use `RequireAuthenticationApiKey` to accept only API key callers. The argument is the claim identification the key must carry:

```csharp
endpoints.MapGet("/integration/data", handler)
    .RequireAuthenticationApiKey("MY_INTEGRATION_CLAIM");
```

Requests authenticated with JWT are rejected with `403 Forbidden`. Unauthenticated requests receive `401 Unauthorized`.

### Mixed JWT + API key (claim-based)

Use `RequireAuthorization()` combined with `RequireAuthorizationClaim` to accept either a valid JWT or a valid API key that carries the given claim:

```csharp
endpoints.MapGet("/shared/data", handler)
    .RequireAuthorization()
    .RequireAuthorizationClaim("MY_CLAIM");
```

### JWT only

Call `RequireAuthorization()` without any API-key-specific filter. Because both schemes are in the default policy, you should also check `User.Identity.AuthenticationType` in the handler if you need to explicitly reject API key callers:

```csharp
if (User.Identity?.AuthenticationType == RbkAuthenticationSchemes.API_KEY)
    return Results.Forbid();
```

---

## Rate Limiting

The library uses ASP.NET Core's built-in **token bucket** rate limiter, configured per API key.

### How it works

1. **Policy load** — `ApiKeyRateLimitPolicyMiddleware` runs early in the pipeline. For every request carrying an `Api-Key` header it looks up (or populates from the database) the key's `RequestsPerMinute` and `BurstLimit` and stores them in `IMemoryCache`.

2. **Partition** — Each key is partitioned by `keyHash + RPM + BurstLimit`. Changing the limits on a key and invalidating the cache forces a new limiter partition to be allocated automatically.

3. **Token bucket** — The bucket starts with `BurstLimit` tokens. Every minute `RequestsPerMinute` tokens are replenished (up to `BurstLimit`). Each accepted request consumes one token. Requests that arrive when the bucket is empty receive **HTTP 429 Too Many Requests**.

### Per-key parameters

| Parameter | Description | Constraints |
|---|---|---|
| `RequestsPerMinute` | Sustained request rate replenished each minute | 1 – 100 000 |
| `BurstLimit` | Maximum token capacity (maximum burst size) | ≥ `RequestsPerMinute` |

### Default values

When `RequestsPerMinute` or `BurstLimit` are omitted at creation time, the library falls back to the values from `RbkBuiltInApiKeyOptions`:

```csharp
.AddApiKeyAuthentication(options =>
{
    options.RequestsPerMinute = 600; // applied to both RPM and BurstLimit when neither is supplied
})
```

If only `RequestsPerMinute` is provided in the create request, `BurstLimit` defaults to the same value.

### Unknown or revoked keys

Keys that are not found, inactive, or expired are still assigned a rate-limit partition using the global default RPM. This prevents abuse through key enumeration while keeping 429 responses consistent.

---

## Management Endpoints

All endpoints require the `CAN_MANAGE_APIKEYS` claim on a JWT token. All request and response bodies are JSON.

---

### `GET /api/authorization/api-keys` — List API Keys

Returns all API keys visible to the caller, filtered by tenant scope.

**Response:** Array of `ApiKeyDetails`

```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "My Integration Key",
    "keyPrefix": "rbk_live_",
    "expirationDate": "2026-12-31T23:59:59Z",
    "isActive": true,
    "createdAt": "2025-01-15T10:00:00Z",
    "lastUsedAt": "2025-06-01T08:30:00Z",
    "tenantId": "ACME",
    "revokeDate": null,
    "revokeReason": null,
    "requestsPerMinute": 600,
    "burstLimit": 1200,
    "assignedClaims": [
      {
        "id": "...",
        "identification": "MY_INTEGRATION",
        "description": "Allow integration access",
        "isProtected": false,
        "allowApiKeyUsage": true
      }
    ]
  }
]
```

---

### `POST /api/authorization/api-keys` — Create API Key

**Request body:**

```json
{
  "name": "My Integration Key",
  "tenantId": "ACME",
  "expirationDate": "2026-12-31T23:59:59Z",
  "requestsPerMinute": 300,
  "burstLimit": 600,
  "claimIds": ["<guid>", "<guid>"]
}
```

| Field | Required | Notes |
|---|---|---|
| `name` | Yes | Max 256 characters |
| `claimIds` | Yes | At least one; all must have `AllowApiKeyUsage = true` |
| `tenantId` | No | Must match caller's tenant unless caller has `CAN_MANAGE_CROSS_TENANT_API_KEYS`; `null` creates a global key |
| `expirationDate` | No | ISO 8601 UTC; no value means the key never expires |
| `requestsPerMinute` | No | Defaults to the configured global default |
| `burstLimit` | No | Defaults to `requestsPerMinute`; must be ≥ `requestsPerMinute` |

**Response:** The created key as `ApiKeyDetails` plus the `rawKey` field. **This is the only time the raw key value is returned — store it securely.**

```json
{
  "id": "...",
  "rawKey": "rbk_live_fedcba9876543210...",
  "keyPrefix": "rbk_live_",
  "name": "My Integration Key",
  ...
}
```

---

### `PUT /api/authorization/api-keys` — Update API Key

Updates metadata, active status, rate limits, and assigned claims. The underlying key value is unchanged.

**Request body:**

```json
{
  "id": "<guid>",
  "name": "Updated Name",
  "isActive": true,
  "expirationDate": "2027-06-01T00:00:00Z",
  "requestsPerMinute": 1000,
  "burstLimit": 2000,
  "claimIds": ["<guid>"]
}
```

| Field | Required | Notes |
|---|---|---|
| `id` | Yes | ID of the key to update |
| `name` | Yes | Max 256 characters |
| `claimIds` | Yes | Replaces the full claim set; all must have `AllowApiKeyUsage = true` |
| `isActive` | Yes | Set to `false` to temporarily disable without revoking |
| `expirationDate` | No | Pass `null` to remove the expiration |
| `requestsPerMinute` | No | Omit to keep the current value |
| `burstLimit` | No | Omit to keep the current value |

Both the auth cache and the rate-limit cache entries for this key are invalidated immediately.

**Response:** Updated `ApiKeyDetails` (no `rawKey` field).

---

### `POST /api/authorization/api-keys/revoke` — Revoke API Key

Permanently deactivates a key, recording a reason and timestamp.

**Request body:**

```json
{
  "id": "<guid>",
  "reason": "Key was compromised"
}
```

| Field | Required | Notes |
|---|---|---|
| `id` | Yes | ID of the key to revoke |
| `reason` | Yes | Max 2048 characters |

A revoked key cannot be re-activated through this endpoint. To re-enable a key use the Update endpoint and set `isActive: true`.

The auth cache entry for this key is invalidated immediately.

**Response:** Updated `ApiKeyDetails` with `isActive: false`, `revokeDate`, and `revokeReason` populated.

---

## Caching

To avoid a database round-trip on every authenticated request, results are held in `IMemoryCache`:

| Entry | Cache key pattern | Invalidated by |
|---|---|---|
| Auth result (claims, tenant, active flag, expiry) | `rbk:apikey:auth:<hash>` | Update, Revoke |
| Rate-limit policy (RPM + burst) | `rbk:apikey:ratelimit:<hash>` | Update, Revoke |
| `LastUsedAt` throttle sentinel | `rbk:apikey:lastused-throttle:<id>` | Expiry only (configurable interval) |

The default cache TTL for auth and rate-limit entries is 10 minutes. During that window a revoked or updated key may still succeed until the cache expires or is explicitly invalidated — Update and Revoke both call `IApiKeyAuthenticationCacheInvalidation.InvalidateByKeyHash` to force immediate eviction.

---

## Usage Tracking

Every successful authentication performs two best-effort write operations that do not block the request if they fail:

**Daily counter** — Increments (or inserts) a row in `ApiKeyUsageByDay` keyed by `(ApiKeyId, Date)`. This gives you a per-day request count for each key.

**Last used timestamp** — Updates `ApiKey.LastUsedAt` at most once per `LastUsedUpdateMinInterval` (default: 2 minutes) to reduce write pressure at high request rates. The throttle is enforced via a short-lived memory cache entry per key ID.

---

## Seeding API Keys

Use `ApiKeySeeding.SeedApiKeyAsync` to insert a known key during application startup or test setup:

```csharp
using rbkApiModules.Identity.Relational;

var result = await ApiKeySeeding.SeedApiKeyAsync(
    context: dbContext,
    name: "Integration Test Key",
    claimIds: new[] { claimId },
    rawKey: "rbk_live_fedcba9876543210fedcba9876543210fedcba9876543210fedcba9876543210",
    expirationDate: null,
    tenantId: null,
    cancellationToken: ct
);

// result.RawKey   — raw value to use in the Api-Key header
// result.ApiKeyId — database ID of the persisted entity
```

Pass `rawKey: null` to have the library generate a new random key. Pass an explicit value (must start with `rbk_live_`) for deterministic keys, which is useful for test fixtures where you want to hardcode the header value.
