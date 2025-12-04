# rbkApiModules.Identity.Core

Comprehensive identity and authentication management with JWT support for ASP.NET Core applications.

## Overview

`rbkApiModules.Identity.Core` provides a complete identity management system with JWT authentication, user management, role-based authorization, and multi-tenant support. It's designed to work seamlessly with the Commons.Core package and provides flexible authentication options.

## Key Components

### Authentication Services

#### IUserAuthenticator
Main authentication service for user login and token generation.

```csharp
public interface IUserAuthenticator
{
    Task<JwtResponse> Authenticate(string username, string tenant, CancellationToken cancellationToken);
}
```

#### JwtFactory
JWT token generation with support for custom claims and tenant switching.

```csharp
public interface IJwtFactory
{
    Task<string> GenerateEncodedTokenAsync(string username, Dictionary<string, string[]> roles, CancellationToken cancellationToken);
}
```

**Features:**
- JWT token generation with configurable expiration
- Support for custom claims and roles
- Tenant-aware token generation
- Refresh token support

### Authentication Handlers

#### ApiKeyAuthenticationHandler
API key-based authentication for service-to-service communication.

```csharp
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync();
}
```

#### MockedWindowsAuthenticationHandler
Windows authentication handler for testing environments.

```csharp
public class MockedWindowsAuthenticationHandler : AuthenticationHandler<TestAuthHandlerOptions>
{
    public const string AuthenticationScheme = "MockedWindows";
}
```

### Core Models

#### User
Core user entity with authentication and authorization properties.

```csharp
public class User : TenantEntity
{
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public string Email { get; set; }
    public string Avatar { get; set; }
    public bool IsConfirmed { get; set; }
    public DateTime? RefreshTokenValidity { get; set; }
    public string RefreshToken { get; set; }
    
    public ICollection<UserToRole> Roles { get; set; }
    public ICollection<UserToClaim> Claims { get; set; }
}
```

#### Role
Role entity for role-based authorization.

```csharp
public class Role : TenantEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsApplicationWide { get; set; }
    
    public ICollection<UserToRole> Users { get; set; }
    public ICollection<RoleToClaim> Claims { get; set; }
}
```

#### Claim
Claim entity for fine-grained authorization.

```csharp
public class Claim : BaseEntity
{
    public string Identification { get; set; }
    public string Description { get; set; }
    public bool Hidden { get; set; }
    public bool Protected { get; set; }
}
```

#### Tenant
Multi-tenant support with tenant isolation.

```csharp
public class Tenant : BaseEntity
{
    public string Alias { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool IsActive { get; set; }
}
```

### JWT Configuration

#### JwtIssuerOptions
Configuration options for JWT token generation.

```csharp
public class JwtIssuerOptions
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public double AccessTokenLife { get; set; }  // in minutes
    public double RefreshTokenLife { get; set; } // in minutes
    public SigningCredentials? SigningCredentials { get; set; }
}
```

#### JwtResponse
Response model for authentication operations.

```csharp
public class JwtResponse
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}
```

### Authentication Options

#### RbkAuthenticationOptions
Comprehensive configuration options for the authentication system.

```csharp
public class RbkAuthenticationOptions
{
    // JWT Configuration
    public RbkAuthenticationOptions UseJwtAuthentication();
    public RbkAuthenticationOptions UseSymmetricEncryptionKey();
    public RbkAuthenticationOptions UseAsymmetricEncryptionKey();
    
    // Tenant Configuration
    public RbkAuthenticationOptions AllowTenantSwitching();
    public RbkAuthenticationOptions DisallowTenantSwitching();
    
    // User Creation
    public RbkAuthenticationOptions AllowUserCreationOnFirstAccess();
    public RbkAuthenticationOptions DisallowUserCreationOnFirstAccess();
    
    // Login Modes
    public RbkAuthenticationOptions UseWindowsAuthentication();
    public RbkAuthenticationOptions UseCustomAuthentication();
    
    // Additional Schemes
    public RbkAuthenticationOptions AddAuthenticationScheme(Action<AuthenticationBuilder> scheme);
}
```

## Configuration

### Basic Setup

```csharp
// Program.cs
builder.Services.AddRbkAuthentication(options =>
{
    options.UseJwtAuthentication()
           .UseSymmetricEncryptionKey()
           .AllowTenantSwitching()
           .AllowUserCreationOnFirstAccess();
});
```

### JWT Configuration

```json
// appsettings.json
{
  "JwtIssuerOptions": {
    "Issuer": "YourApplication",
    "Audience": "YourApplication",
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

### Advanced Configuration

```csharp
builder.Services.AddRbkAuthentication(options =>
{
    options.UseJwtAuthentication()
           .UseSymmetricEncryptionKey()
           .AllowTenantSwitching()
           .AllowUserCreationOnFirstAccess()
           .UseWindowsAuthentication()
           .AddAuthenticationScheme(auth =>
           {
               auth.AddApiKey("ApiKey", configureOptions: null);
           });
});
```

## Authentication Modes

### JWT Authentication
Standard JWT-based authentication with token refresh support.

```csharp
// Login endpoint
[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginRequest request)
{
    var response = await _userAuthenticator.Authenticate(
        request.Username, 
        request.Tenant, 
        CancellationToken.None);
    
    return Ok(response);
}
```

### Windows Authentication
Integration with Windows authentication for enterprise environments.

```csharp
// Configure Windows authentication
options.UseWindowsAuthentication()
       .AllowUserCreationOnFirstAccess();
```

### API Key Authentication
For service-to-service communication or machine-to-machine authentication.

```csharp
// Add API key authentication
options.AddAuthenticationScheme(auth =>
{
    auth.AddApiKey("ApiKey", configureOptions: null);
});

// Use in controller
[Authorize(AuthenticationSchemes = "ApiKey")]
public class ApiController : ControllerBase
{
    // API key protected endpoints
}
```

## Multi-Tenant Support

### Tenant-Aware Authentication
Automatic tenant filtering and isolation.

```csharp
// Tenant switching in JWT tokens
options.AllowTenantSwitching();

// Tenant-aware user creation
options.AllowUserCreationOnFirstAccess();
```

### Tenant Management
Built-in tenant management capabilities.

```csharp
public interface ITenantsService
{
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken);
    Task<Tenant> FindAsync(string alias, CancellationToken cancellationToken);
    Task<Tenant[]> GetAllAsync(CancellationToken cancellationToken);
    Task DeleteAsync(string alias, CancellationToken cancellationToken);
}
```

## User Management

### User Authentication
Complete user authentication flow with refresh tokens.

```csharp
public interface IAuthService
{
    Task<User> FindUserAsync(string username, string tenant, CancellationToken cancellationToken);
    Task<User> GetUserWithDependenciesAsync(string username, string tenant, CancellationToken cancellationToken);
    Task ConfirmUserAsync(string username, string tenant, CancellationToken cancellationToken);
    Task UpdateRefreshTokenAsync(string username, string tenant, string refreshToken, double validityMinutes, CancellationToken cancellationToken);
}
```

### Role-Based Authorization
Flexible role and claim management.

```csharp
public interface IRolesService
{
    Task<Role> CreateAsync(Role role, CancellationToken cancellationToken);
    Task<Role> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<Role[]> FindByNameAsync(string name, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
```

## Claims System

### Custom Claims
Extensible claims system for fine-grained authorization.

```csharp
public interface IClaimsService
{
    Task<Claim[]> GetAllAsync(CancellationToken cancellationToken);
    Task<Claim> FindAsync(Guid id, CancellationToken cancellationToken);
    Task<Claim> FindByIdentificationAsync(string identification, CancellationToken cancellationToken);
    Task<Claim> CreateAsync(Claim claim, CancellationToken cancellationToken);
}
```

### Claim Handlers
Custom claim processing for dynamic authorization.

```csharp
public interface ICustomClaimHandler
{
    Task<Dictionary<string, string[]>> ProcessClaimsAsync(User user, CancellationToken cancellationToken);
}
```

## Email Templates

### Built-in Templates
HTML and text email templates for user management.

- `email-confirmation.html` / `email-confirmation.txt`
- `email-confirmed.html` / `email-confirmed.txt`
- `email-reset.html` / `email-reset.txt`
- `reset-confirmed.html` / `reset-confirmed.txt`

### Template Usage
Templates are embedded as resources and can be customized for your application.

## Testing Support

### Mocked Authentication
Built-in support for testing environments.

```csharp
// Automatically enabled in testing environments
if (TestingEnvironmentChecker.IsTestingEnvironment)
{
    // Mocked authentication is automatically configured
}
```

## Dependencies

- Microsoft.AspNetCore.App (Framework Reference)
- FluentValidation
- Microsoft.AspNetCore.Authentication.JwtBearer

## Best Practices

1. **Security**: Use strong secret keys and rotate them regularly
2. **Token Life**: Set appropriate token lifetimes based on your security requirements
3. **Tenant Isolation**: Always use tenant-aware queries in multi-tenant applications
4. **Claims**: Use claims for fine-grained authorization rather than roles alone
5. **Refresh Tokens**: Implement proper refresh token rotation for security

## Examples

### Complete Authentication Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Configure authentication
builder.Services.AddRbkAuthentication(options =>
{
    options.UseJwtAuthentication()
           .UseSymmetricEncryptionKey()
           .AllowTenantSwitching()
           .AllowUserCreationOnFirstAccess();
});

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("role", "admin"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### Authentication Controller

```csharp
[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly IUserAuthenticator _authenticator;
    
    public AuthenticationController(IUserAuthenticator authenticator)
    {
        _authenticator = authenticator;
    }
    
    [HttpPost("login")]
    [AllowAnonymous]
    [Produces<JwtResponse>]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authenticator.Authenticate(
            request.Username, 
            request.Tenant, 
            CancellationToken.None);
        
        return Ok(response);
    }
    
    [HttpPost("refresh")]
    [AllowAnonymous]
    [Produces<JwtResponse>]
    public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
    {
        // Implement refresh token logic
        return Ok();
    }
}
```

### Protected Controller

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet]
    [Produces<UserDetails[]>]
    public async Task<IActionResult> GetUsers()
    {
        // Only authenticated users can access
        return Ok();
    }
    
    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    [Produces<CreateUserResponse>]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        // Only admin users can access
        return Ok();
    }
}
``` 