# Identity & Authentication Setup

Complete guide to setting up authentication in rbkApiModules: JWT tokens, Windows Authentication, API keys, and multi-tenancy.

## Overview

rbkApiModules provides flexible authentication out of the box:
- **JWT Authentication** - Token-based auth with refresh tokens
- **Windows Authentication** - Active Directory integration for enterprise environments
- **API Key Authentication** - Machine-to-machine communication ([detailed docs](../README-ApiKeys.md))
- **Multi-Tenancy** - Isolate users and data by tenant

All authentication modes can be combined. For example, JWT for users + API keys for service integrations.

---

## Basic JWT Setup

### 1. Configure appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=myapp.db"
  },
  "JwtIssuerOptions": {
    "Issuer": "MyApplication",
    "Audience": "MyApplication",
    "SecretKey": "your-super-secret-key-minimum-32-characters-long-for-HS256",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `Issuer` | string | *required* | Token issuer identifier (typically your app name) |
| `Audience` | string | *required* | Token audience identifier (typically your app name) |
| `SecretKey` | string | *required* | Secret key for HMAC-SHA256 signing (min 32 chars) |
| `AccessTokenLife` | number | `60` | Access token lifetime in minutes |
| `RefreshTokenLife` | number | `1440` | Refresh token lifetime in minutes (default: 24 hours) |

**Security:** Never commit production secret keys. Use environment variables or secrets management (Azure Key Vault, AWS Secrets Manager, etc.).

### 2. Add Identity Services

```csharp
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add rbk identity services
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
);
```

### 3. Use Identity Middleware

```csharp
var app = builder.Build();

// Apply middleware (includes authentication + authorization)
app.UseRbkRelationalAuthentication();

// Setup database and default admin
app.SetupDatabase<ApplicationDbContext>(options => options.MigrateOnStartup());
app.SetupRbkAuthenticationClaims();
app.SetupRbkDefaultAdmin(options => options
    .WithUsername("admin")
    .WithPassword("admin123")
    .WithEmail("admin@example.com")
);

app.Run();
```

That's it! You now have JWT authentication with a default admin user.

---

## Authentication Configuration Options

### RbkAuthenticationOptions

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    // Encryption
    .UseSymetricEncryptationKey()        // Use HS256 (symmetric)
    .UseAsymmetricEncryptationKey()      // Use RS256 (asymmetric) - requires cert

    // User Creation
    .AllowUserCreationByAdmins()         // Admins can create users via API
    .AllowUserSelfRegistration()         // Public user registration endpoint

    // Tenant Features
    .AllowTenantSwitching()              // Users can switch between tenants
    .AllowAnonymousTenantAccess()        // GET /api/authorization/tenants doesn't require auth

    // Login Modes
    .UseLoginWithWindowsAuthentication() // Enable Windows/NTLM login
    .UseMockedWindowsAuthentication()    // Mock Windows auth for testing

    // Feature Toggles
    .AddApiKeyAuthentication()           // Enable API key support
    .DisablePasswordReset()              // Remove password reset endpoints
    .DisableEmailConfirmation()          // Remove email confirmation endpoints
    .DisableRefreshToken()               // Remove refresh token endpoint

    // Custom Actions
    .WithTenantPostCreationAction<MyTenantAction>()  // Run custom logic after tenant creation
);
```

---

## Multi-Tenancy

### What is Multi-Tenancy?

Multi-tenancy isolates data by tenant. Each user belongs to a tenant, and entities marked as `TenantEntity` are automatically filtered to the user's tenant.

### Enable Multi-Tenancy

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AllowTenantSwitching()  // ← Users can switch tenants if they have access
);
```

### Mark Entities as Tenant-Scoped

```csharp
using rbkApiModules.Commons.Core;

public class Product : TenantEntity  // ← Inherits TenantId property
{
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

**Automatic filtering:** Queries against `Product` will only return records where `TenantId` matches the authenticated user's tenant. No manual filtering required.

### Create Tenant-Specific Users

```bash
POST /api/authorization/tenants
{
  "alias": "acme",
  "name": "Acme Corp",
  "description": "Acme Corporation tenant"
}
```

```bash
POST /api/authorization/users
{
  "username": "john",
  "email": "john@acme.com",
  "password": "password123",
  "tenantId": "acme"
}
```

### Login with Tenant

```bash
POST /api/authentication/login
{
  "username": "john",
  "password": "password123",
  "tenant": "acme"
}
```

The JWT token now includes the tenant claim. All database queries are automatically scoped to "acme".

### Switch Tenant (if enabled)

```bash
POST /api/authentication/switch-tenant
{
  "tenant": "other-tenant"
}
```

Requires the user to have access to the target tenant.

### Global (Cross-Tenant) Users

Users with `tenantId: null` are **global users** (typically admins). They are not tenant-scoped. Use this sparingly for superuser accounts.

---

## Windows Authentication

### Overview

Windows Authentication integrates with Active Directory (NTLM/Kerberos). When enabled:
- Users authenticate via Windows credentials
- Password-related endpoints (reset, change password) are disabled
- Users are auto-created on first login if configured

### Enable Windows Authentication

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .UseLoginWithWindowsAuthentication()
);
```

### Login Endpoint

```bash
POST /api/authentication/login/windows
# No body required - uses Windows credentials from HTTP context
```

Or for combined mode (Windows + credentials):

```bash
POST /api/authentication/login
{
  "username": "john",
  "password": "password123",
  "tenant": null
}
```

### Auto-Create Users on First Access

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .UseLoginWithWindowsAuthentication()
    .AllowUserCreationByAdmins()  // ← Auto-creates users on first Windows login
);
```

When a Windows user logs in for the first time, a User record is created automatically with their username.

### Testing Windows Auth Locally

Use the mocked handler in test/dev environments:

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .UseLoginWithWindowsAuthentication()
    .UseMockedWindowsAuthentication()  // ← Mock for testing
);
```

---

## API Key Authentication

See **[README-ApiKeys.md](../README-ApiKeys.md)** for comprehensive API key documentation, including:
- Setup and configuration
- Creating and managing API keys
- Rate limiting
- Claims and permissions
- Management endpoints

Quick enable:

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AddApiKeyAuthentication()  // ← Enable API keys
);
```

---

## Endpoint Protection

### Require JWT Authentication

```csharp
app.MapGet("/api/data", GetData)
    .RequireAuthorization()
    .Produces<DataResponse>();
```

### Require Specific Claim (JWT or API Key)

```csharp
app.MapPost("/api/data", CreateData)
    .RequireAuthorization()
    .RequireAuthorizationClaim("CREATE_DATA")
    .Produces<DataResponse>();
```

### Require API Key Only

```csharp
app.MapPost("/api/webhook", ProcessWebhook)
    .RequireAuthenticationApiKey("WEBHOOK_ACCESS")
    .Produces();
```

This endpoint **only** accepts API keys, not JWT tokens.

### Allow Anonymous

```csharp
app.MapGet("/api/health", () => "OK")
    .AllowAnonymous()
    .Produces<string>();
```

---

## Token Refresh

### Refresh Token Flow

1. User logs in → receives `accessToken` and `refreshToken`
2. Access token expires (default: 60 minutes)
3. Client calls refresh endpoint with `refreshToken`
4. Receives new `accessToken` and `refreshToken`

### Refresh Endpoint

```bash
POST /api/authentication/renew
{
  "refreshToken": "YOUR_REFRESH_TOKEN"
}
```

Response:

```json
{
  "accessToken": "NEW_ACCESS_TOKEN",
  "refreshToken": "NEW_REFRESH_TOKEN"
}
```

### Disable Refresh Tokens

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .DisableRefreshToken()  // ← Remove refresh endpoint
);
```

---

## User Self-Registration

### Enable Public Registration

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AllowUserSelfRegistration()  // ← Public registration endpoint
);
```

### Registration Endpoint

```bash
POST /api/authentication/register
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "password123",
  "tenantId": "acme"
}
```

**Note:** Cannot be used with Windows Authentication.

---

## Email Confirmation

### Overview

When enabled, users must confirm their email before logging in.

### Enable Email Confirmation

Email confirmation is enabled by default. Disable it if you don't want it:

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .DisableEmailConfirmation()  // ← Disable
);
```

### Confirmation Flow

1. User registers → receives confirmation email
2. User clicks link → `GET /api/authentication/confirm-email?token=...`
3. Email confirmed → user can log in

### Resend Confirmation Email

```bash
POST /api/authentication/resend-confirmation
{
  "username": "john",
  "tenant": "acme"
}
```

---

## Password Reset

### Enable Password Reset

Enabled by default. Disable if not needed:

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .DisablePasswordReset()  // ← Disable
);
```

### Password Reset Flow

1. User requests reset → `POST /api/authentication/request-reset`
2. User receives reset email with token
3. User submits new password → `POST /api/authentication/redefine-password`

### Request Reset

```bash
POST /api/authentication/request-reset
{
  "username": "john",
  "tenant": "acme"
}
```

### Redefine Password

```bash
POST /api/authentication/redefine-password
{
  "token": "RESET_TOKEN",
  "newPassword": "newpassword123"
}
```

---

## Change Password (Authenticated)

```bash
POST /api/authentication/change-password
Authorization: Bearer YOUR_JWT_TOKEN

{
  "currentPassword": "oldpassword",
  "newPassword": "newpassword123"
}
```

---

## Custom Post-Creation Actions

### Tenant Post-Creation Hook

Run custom logic after a tenant is created:

```csharp
public class MyTenantPostCreationAction : ITenantPostCreationAction
{
    public async Task ExecuteAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        // Seed tenant-specific data, send notification, etc.
    }
}

builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .WithTenantPostCreationAction<MyTenantPostCreationAction>()
);
```

---

## Built-In Endpoints

When you call `app.UseRbkRelationalAuthentication()`, these endpoints are automatically registered:

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/authentication/login` | Login with username/password |
| POST | `/api/authentication/login/windows` | Login with Windows credentials |
| POST | `/api/authentication/register` | Public user registration |
| POST | `/api/authentication/renew` | Refresh access token |
| POST | `/api/authentication/change-password` | Change password (authenticated) |
| POST | `/api/authentication/request-reset` | Request password reset |
| POST | `/api/authentication/redefine-password` | Redefine password with reset token |
| POST | `/api/authentication/confirm-email` | Confirm user email |
| POST | `/api/authentication/resend-confirmation` | Resend confirmation email |
| POST | `/api/authentication/switch-tenant` | Switch tenant (if enabled) |
| GET  | `/api/authorization/tenants` | List all tenants |

See **[Built-In Endpoints](built-in-endpoints.md)** for complete API reference.

---

## Security Best Practices

1. **Secret Keys:** Use minimum 32 characters for HS256. Rotate keys periodically.
2. **HTTPS:** Always use HTTPS in production. JWT tokens are bearer tokens.
3. **Token Lifetimes:** Keep access tokens short (60 min), refresh tokens longer (24 hrs).
4. **Refresh Token Rotation:** New refresh tokens are issued on every refresh.
5. **Password Policy:** Enforce strong passwords in your validators.
6. **Claims Principle:** Use claims for fine-grained permissions, not just roles.

---

## Troubleshooting

### "Unauthorized" (401) on protected endpoints

- Verify you're sending the JWT token: `Authorization: Bearer YOUR_TOKEN`
- Check token expiration
- Ensure `app.UseRbkRelationalAuthentication()` is called

### "Forbidden" (403) on claim-protected endpoints

- User doesn't have the required claim
- Check user's roles and claims: `GET /api/authorization/users`

### Refresh token returns 401

- Refresh token expired
- User logged out or token was revoked
- User must log in again

### Windows Authentication not working

- Ensure IIS/Kestrel is configured for Windows Authentication
- Check `UseLoginWithWindowsAuthentication()` is called
- Verify network allows NTLM/Kerberos

---

## Next Steps

- **[Identity Management](identity-management.md)** - Manage users, roles, claims, tenants
- **[API Keys](../README-ApiKeys.md)** - Set up machine-to-machine authentication
- **[Built-In Endpoints](built-in-endpoints.md)** - Complete endpoint reference
- **[Testing](Testing.md)** - Write tests with authentication
