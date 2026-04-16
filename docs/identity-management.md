# Identity Management: Users, Roles, Claims & Tenants

Complete guide to managing users, roles, claims, and tenants in rbkApiModules.

## Overview

rbkApiModules provides a complete identity system with built-in API endpoints for managing:
- **Users** - Create, list, activate/deactivate, delete users
- **Roles** - Create roles, assign claims to roles, assign roles to users
- **Claims** - Fine-grained permissions for authorization
- **Tenants** - Multi-tenant isolation and management

All management endpoints require authentication and specific claims. The default admin user has all management claims.

---

## Users

### User Model

```csharp
public class User : TenantEntity
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string? Avatar { get; set; }
    public bool IsConfirmed { get; set; }
    public bool IsActive { get; set; }
    public string? TenantId { get; set; }
    public AuthenticationMode AuthenticationMode { get; set; }  // Credentials or Windows
    
    public ICollection<UserToRole> Roles { get; set; }
    public ICollection<UserToClaim> Claims { get; set; }
}
```

### List All Users

**Endpoint:** `GET /api/authorization/users`

**Required Claim:** `MANAGE_USERS`

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "username": "john",
    "displayName": "John Doe",
    "email": "john@example.com",
    "avatar": null,
    "tenantId": "acme",
    "isActive": true,
    "isConfirmed": true,
    "authenticationMode": "Credentials",
    "roles": [
      {
        "id": "...",
        "name": "Manager"
      }
    ],
    "claims": [
      {
        "id": "...",
        "identification": "READ_DATA",
        "description": "Read application data",
        "accessType": "Allow"
      }
    ]
  }
]
```

### Create User (Admin)

**Endpoint:** `POST /api/authorization/users`

**Required Claim:** `MANAGE_USERS`

**Request:**
```json
{
  "username": "jane",
  "email": "jane@example.com",
  "password": "password123",
  "displayName": "Jane Smith",
  "tenantId": "acme",
  "avatar": null
}
```

**Response:** User details (same as GET response)

**Notes:**
- Available only when `AllowUserCreationByAdmins()` is enabled
- Password is hashed before storage
- User starts as active and unconfirmed (unless email confirmation is disabled)

### Register User (Self-Registration)

**Endpoint:** `POST /api/authentication/register`

**Required Claim:** None (anonymous)

**Request:**
```json
{
  "username": "newuser",
  "email": "newuser@example.com",
  "password": "password123",
  "tenantId": "acme"
}
```

**Response:** User details

**Notes:**
- Available only when `AllowUserSelfRegistration()` is enabled
- Cannot be used with Windows Authentication
- User receives confirmation email if email confirmation is enabled

### Activate User

**Endpoint:** `POST /api/authorization/users/activate`

**Required Claim:** `MANAGE_USERS`

**Request:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Deactivate User

**Endpoint:** `POST /api/authorization/users/deactivate`

**Required Claim:** `MANAGE_USERS`

**Request:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

Deactivated users cannot log in until reactivated.

### Delete User

**Endpoint:** `POST /api/authorization/users/delete`

**Required Claim:** `MANAGE_USERS`

**Request:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Warning:** This is a permanent deletion. Consider deactivating instead.

---

## Roles

### Role Model

```csharp
public class Role : TenantEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsApplicationWide { get; set; }  // Global role (not tenant-specific)
    
    public ICollection<UserToRole> Users { get; set; }
    public ICollection<RoleToClaim> Claims { get; set; }
}
```

### List All Roles

**Endpoint:** `GET /api/authorization/roles`

**Required Claim:** `MANAGE_TENANT_SPECIFIC_ROLES` or `MANAGE_APPLICATION_WIDE_ROLES`

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "name": "Manager",
    "description": "Managers can view and edit data",
    "tenantId": "acme",
    "isApplicationWide": false,
    "claims": [
      {
        "id": "...",
        "identification": "READ_DATA",
        "description": "Read application data"
      }
    ]
  }
]
```

### Create Role

**Endpoint:** `POST /api/authorization/roles`

**Required Claim:** `MANAGE_TENANT_SPECIFIC_ROLES` (for tenant roles) or `MANAGE_APPLICATION_WIDE_ROLES` (for global roles)

**Request:**
```json
{
  "name": "Manager",
  "description": "Managers can view and edit data",
  "tenantId": "acme",
  "isApplicationWide": false
}
```

**Response:** Role details

**Notes:**
- Tenant-specific roles: `tenantId` is set, `isApplicationWide: false`
- Global roles: `tenantId: null`, `isApplicationWide: true`
- Global roles require `MANAGE_APPLICATION_WIDE_ROLES` claim

### Rename Role

**Endpoint:** `POST /api/authorization/roles/rename`

**Required Claim:** `MANAGE_TENANT_SPECIFIC_ROLES` or `MANAGE_APPLICATION_WIDE_ROLES`

**Request:**
```json
{
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "newName": "Senior Manager",
  "newDescription": "Senior managers with elevated permissions"
}
```

### Update Role Claims

**Endpoint:** `POST /api/authorization/roles/claims`

**Required Claim:** `MANAGE_TENANT_SPECIFIC_ROLES` or `MANAGE_APPLICATION_WIDE_ROLES`

**Request:**
```json
{
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "claimIds": [
    "claim-id-1",
    "claim-id-2"
  ]
}
```

**Notes:**
- Replaces the full claim list for the role
- Users with this role inherit all assigned claims

### Delete Role

**Endpoint:** `POST /api/authorization/roles/delete`

**Required Claim:** `MANAGE_TENANT_SPECIFIC_ROLES` or `MANAGE_APPLICATION_WIDE_ROLES`

**Request:**
```json
{
  "roleId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

---

## Claims

### Claim Model

```csharp
public class Claim : BaseEntity
{
    public string Identification { get; set; }  // e.g. "READ_DATA"
    public string Description { get; set; }
    public bool Hidden { get; set; }            // Hidden from UI
    public bool Protected { get; set; }         // Cannot be deleted
    public bool AllowApiKeyUsage { get; set; }  // Can be assigned to API keys
}
```

### List All Claims

**Endpoint:** `GET /api/authorization/claims`

**Required Claim:** `MANAGE_CLAIMS`

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "identification": "READ_DATA",
    "description": "Read application data",
    "hidden": false,
    "protected": false,
    "allowApiKeyUsage": true
  }
]
```

### Create Claim

**Endpoint:** `POST /api/authorization/claims`

**Required Claim:** `MANAGE_CLAIMS`

**Request:**
```json
{
  "identification": "READ_DATA",
  "description": "Read application data",
  "allowApiKeyUsage": true
}
```

**Response:** Claim details

**Notes:**
- `identification` must be unique and typically UPPER_SNAKE_CASE
- Set `allowApiKeyUsage: true` if you want to assign this claim to API keys

### Update Claim

**Endpoint:** `POST /api/authorization/claims/update`

**Required Claim:** `MANAGE_CLAIMS`

**Request:**
```json
{
  "claimId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "description": "Updated description",
  "allowApiKeyUsage": true
}
```

**Notes:**
- Cannot update `identification` (it's the claim's identity)
- Cannot update `protected` or `hidden` via this endpoint (use protect/unprotect)

### Protect Claim

**Endpoint:** `POST /api/authorization/claims/protect`

**Required Claim:** `CHANGE_CLAIM_PROTECTION`

**Request:**
```json
{
  "claimId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

Protected claims cannot be deleted. Use this for system-critical claims.

### Unprotect Claim

**Endpoint:** `POST /api/authorization/claims/unprotect`

**Required Claim:** `CHANGE_CLAIM_PROTECTION`

**Request:**
```json
{
  "claimId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

### Delete Claim

**Endpoint:** `POST /api/authorization/claims/delete`

**Required Claim:** `MANAGE_CLAIMS`

**Request:**
```json
{
  "claimId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Notes:**
- Cannot delete protected claims
- Deleting a claim removes it from all users and roles

---

## User-Role Assignment

### Replace User Roles

**Endpoint:** `POST /api/authorization/users/roles`

**Required Claim:** `MANAGE_USER_ROLES`

**Request:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "roleIds": [
    "role-id-1",
    "role-id-2"
  ]
}
```

**Response:** Updated user details

**Notes:**
- Replaces the full role list for the user
- User inherits all claims from assigned roles

---

## Claim Overrides (Direct User Claims)

Users can have claims assigned directly (not via roles). This is useful for:
- Granting extra permissions to specific users
- Denying permissions that would otherwise be inherited from roles

### Add Claim Override

**Endpoint:** `POST /api/authorization/users/claims`

**Required Claim:** `OVERRIDE_USER_CLAIMS`

**Request:**
```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "claimId": "claim-id",
  "accessType": "Allow"
}
```

**Access Types:**
- `Allow` - User has this claim (even if not in any role)
- `Deny` - User does not have this claim (overrides role assignments)

### Example Use Case: Deny Inherited Claim

User is in "Manager" role which includes `DELETE_DATA` claim. You want to prevent this specific user from deleting:

```json
{
  "userId": "user-id",
  "claimId": "delete-data-claim-id",
  "accessType": "Deny"
}
```

Now the user cannot delete data, even though their role says they can.

---

## Tenants

### Tenant Model

```csharp
public class Tenant : BaseEntity
{
    public string Alias { get; set; }        // Unique identifier (e.g. "acme")
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}
```

### List All Tenants

**Endpoint:** `GET /api/authorization/tenants`

**Required Claim:** None (if `AllowAnonymousTenantAccess()` is enabled), otherwise `MANAGE_TENANTS`

**Response:**
```json
[
  {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "alias": "acme",
    "name": "Acme Corporation",
    "description": "Main tenant for Acme Corp",
    "isActive": true
  }
]
```

### Create Tenant

**Endpoint:** `POST /api/authorization/tenants`

**Required Claim:** `MANAGE_TENANTS`

**Request:**
```json
{
  "alias": "acme",
  "name": "Acme Corporation",
  "description": "Main tenant for Acme Corp"
}
```

**Response:** Tenant details

**Notes:**
- `alias` must be unique and URL-safe (typically lowercase, no spaces)
- After creation, `ITenantPostCreationAction` hook runs (if configured)

### Update Tenant

**Endpoint:** `POST /api/authorization/tenants/update`

**Required Claim:** `MANAGE_TENANTS`

**Request:**
```json
{
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "Acme Corp",
  "description": "Updated description",
  "isActive": true
}
```

**Notes:**
- Cannot update `alias` (it's the tenant's identifier)
- Setting `isActive: false` disables the tenant (users cannot log in)

### Delete Tenant

**Endpoint:** `POST /api/authorization/tenants/delete`

**Required Claim:** `MANAGE_TENANTS`

**Request:**
```json
{
  "tenantId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Warning:** This deletes the tenant and all associated tenant-scoped data (users, roles, entities). Use with extreme caution.

---

## Built-In Claims

rbkApiModules ships with these protected claims for identity management:

| Claim Identification | Description | Required For |
|---------------------|-------------|--------------|
| `MANAGE_TENANTS` | Manage tenants | Tenant CRUD operations |
| `MANAGE_USERS` | Manage users | User CRUD operations |
| `MANAGE_USER_ROLES` | Manage user roles | Assign roles to users |
| `OVERRIDE_USER_CLAIMS` | Override user claims | Add/remove direct claims on users |
| `MANAGE_CLAIMS` | Manage claims | Claim CRUD operations |
| `CHANGE_CLAIM_PROTECTION` | Change claim protection | Protect/unprotect claims |
| `MANAGE_TENANT_SPECIFIC_ROLES` | Manage tenant roles | Tenant-scoped role CRUD |
| `MANAGE_APPLICATION_WIDE_ROLES` | Manage global roles | Global role CRUD |
| `CAN_MANAGE_APIKEYS` | Manage API keys | API key CRUD operations |
| `CAN_MANAGE_CROSS_TENANT_API_KEYS` | Manage cross-tenant API keys | Create global API keys |

These claims are automatically seeded when you call `app.SetupRbkAuthenticationClaims()`.

---

## Default Admin Setup

### Configure Default Admin

```csharp
app.SetupRbkDefaultAdmin(options => options
    .WithUsername("superuser")
    .WithPassword("admin123")
    .WithDisplayName("System Administrator")
    .WithEmail("admin@example.com")
    .WithAvatar(null)
);
```

The default admin user is created if it doesn't exist. It has:
- All built-in management claims
- No tenant (`tenantId: null`) — global user
- Active and confirmed

**Security:** Change the default password immediately in production.

---

## Custom Claims Setup

Customize the descriptions of built-in claims:

```csharp
app.SetupRbkAuthenticationClaims(options => options
    .WithCustomDescription(x => x.ManageTenants, "Manage company tenants")
    .WithCustomDescription(x => x.ManageUsers, "Manage application users")
    .WithCustomDescription(x => x.ManageClaims, "Manage security claims")
    .WithCustomDescription(x => x.ManageApiKeys, "Manage API key authentication")
);
```

---

## Common Workflows

### Create a New User with Roles

1. Create user:
```bash
POST /api/authorization/users
{
  "username": "john",
  "email": "john@example.com",
  "password": "password123",
  "tenantId": "acme"
}
```

2. Assign roles:
```bash
POST /api/authorization/users/roles
{
  "userId": "USER_ID_FROM_STEP_1",
  "roleIds": ["MANAGER_ROLE_ID", "VIEWER_ROLE_ID"]
}
```

### Create a Role with Claims

1. Create role:
```bash
POST /api/authorization/roles
{
  "name": "Data Manager",
  "description": "Manage application data",
  "tenantId": "acme"
}
```

2. Assign claims to role:
```bash
POST /api/authorization/roles/claims
{
  "roleId": "ROLE_ID_FROM_STEP_1",
  "claimIds": ["READ_DATA_CLAIM_ID", "WRITE_DATA_CLAIM_ID"]
}
```

### Grant User Extra Permission

User has "Viewer" role but needs temporary write access:

```bash
POST /api/authorization/users/claims
{
  "userId": "USER_ID",
  "claimId": "WRITE_DATA_CLAIM_ID",
  "accessType": "Allow"
}
```

### Revoke Inherited Permission

User is in "Manager" role but shouldn't be able to delete:

```bash
POST /api/authorization/users/claims
{
  "userId": "USER_ID",
  "claimId": "DELETE_DATA_CLAIM_ID",
  "accessType": "Deny"
}
```

---

## Authorization in Your Code

### Check Claims in Endpoint Filter

```csharp
app.MapPost("/api/data", CreateData)
    .RequireAuthorization()
    .RequireAuthorizationClaim("CREATE_DATA")
    .Produces<DataResponse>();
```

### Check Claims in Handler

```csharp
public class CreateDataHandler : IRequestHandler<CreateDataRequest, CreateDataResponse>
{
    public async Task<CreateDataResponse> HandleAsync(
        CreateDataRequest request, 
        CancellationToken cancellationToken)
    {
        if (!request.Identity.HasClaim("CREATE_DATA"))
        {
            throw new UnauthorizedAccessException("Missing CREATE_DATA claim");
        }
        
        // Process request
    }
}
```

### Get Current User Info

```csharp
public class CreateDataRequest : AuthenticatedRequest
{
    // Request properties
}

// In handler:
var username = request.Identity.Username;
var tenant = request.Identity.Tenant;
var claims = request.Identity.Claims;
```

---

## Testing Identity Management

### Test User Creation

```csharp
public class UserManagementTests : RbkTestingServer<Program>
{
    [Test]
    public async Task CreateUser_ReturnsSuccess()
    {
        // Arrange
        await CacheCredentialsAsync("admin", "admin123", null);
        
        // Act
        var response = await PostAsync<UserDetails>(
            "/api/authorization/users",
            new CreateUserRequest
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123",
                TenantId = null
            },
            "admin"
        );
        
        // Assert
        response.ShouldBeSuccess(out var user);
        user.Username.ShouldBe("testuser");
    }
}
```

---

## Next Steps

- **[Authentication Setup](identity-authentication.md)** - Configure JWT, Windows Auth, API keys
- **[API Keys](../README-ApiKeys.md)** - Machine-to-machine authentication
- **[Built-In Endpoints](built-in-endpoints.md)** - Complete endpoint reference
- **[Testing](Testing.md)** - Test identity features
