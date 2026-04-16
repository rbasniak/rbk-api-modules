# Getting Started with rbkApiModules

A quick-start guide to get your ASP.NET Core Web API up and running with rbkApiModules in under 10 minutes.

## Prerequisites

- .NET 10 SDK
- A SQL database (SQLite, SQL Server, PostgreSQL, etc.)
- Visual Studio 2022, VS Code, or Rider

## Step 1: Install Packages

Add the core packages to your ASP.NET Core Web API project:

```bash
# Core infrastructure
dotnet add package rbkApiModules.Commons.Core

# Identity and authentication
dotnet add package rbkApiModules.Identity.Core

# Testing framework (for test projects)
dotnet add package rbkApiModules.Commons.Testing

# Code analyzers (optional but recommended)
dotnet add package rbkApiModules.Analysers
```

## Step 2: Configure appsettings.json

Add your database connection string and JWT configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=myapp.db"
  },
  "JwtIssuerOptions": {
    "Issuer": "MyApplication",
    "Audience": "MyApplication",
    "SecretKey": "your-super-secret-key-minimum-32-characters-long",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

**Security Note:** Never commit real secret keys to source control. Use environment variables or Azure Key Vault in production.

## Step 3: Create Your DbContext

Create a DbContext that includes the identity entities:

```csharp
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Identity.Core;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply identity configurations
        modelBuilder.ApplyRbkIdentityConfigurations();
        
        // Your application entities
        // modelBuilder.Entity<YourEntity>().ToTable("YourEntities");
    }
}
```

## Step 4: Configure Services in Program.cs

Set up rbkApiModules in your `Program.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using rbkApiModules.Commons.Core.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Get connection string (with test database isolation support)
string connectionString;
if (TestingEnvironmentChecker.IsTestingEnvironment)
{
    connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection")!
        .Replace("**CONTEXT**", $"Testing.{Guid.NewGuid():N}");
}
else
{
    connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection")!
        .Replace("**CONTEXT**", "Application");
}

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => 
    options.UseSqlite(connectionString));

// Add core rbk services
builder.Services.AddRbkApiCoreSetup(options => options
    .UseDefaultCompression()
    .UseDefaultCors()
    .UseDefaultHsts(builder.Environment.IsDevelopment())
    .UseDefaultHttpsRedirection()
    .UseDefaultMemoryCache()
    .UseDefaultHttpClient()
    .UseHttpContextAccessor()
    .RegisterDbContext<ApplicationDbContext>()
);

// Add identity services
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AllowUserCreationByAdmins()
);

// Add OpenAPI (optional)
builder.Services.AddOpenApi();

var app = builder.Build();

// Use rbk middleware
app.UseRbkApiCoreSetup();
app.UseRbkRelationalAuthentication();

// Setup database (migrations + seed)
app.SetupDatabase<ApplicationDbContext>(options => options
    .MigrateOnStartup()
);

// Setup authentication claims
app.SetupRbkAuthenticationClaims(options => options
    .WithCustomDescription(x => x.ManageTenants, "Manage tenants")
    .WithCustomDescription(x => x.ManageUsers, "Manage users")
    .WithCustomDescription(x => x.ManageClaims, "Manage claims")
    .WithCustomDescription(x => x.ManageUserRoles, "Manage user roles")
    .WithCustomDescription(x => x.ManageApiKeys, "Manage API keys")
    .WithCustomDescription(x => x.ManageCrossTenantApiKeys, "Manage cross-tenant API keys")
);

// Setup default admin user
app.SetupRbkDefaultAdmin(options => options
    .WithUsername("admin")
    .WithPassword("admin123")
    .WithDisplayName("Administrator")
    .WithEmail("admin@example.com")
);

// Map your endpoints here
// app.MapGet("/api/hello", () => "Hello World").RequireAuthorization();

// Map OpenAPI (optional)
app.MapOpenApi().AllowAnonymous();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/openapi/v1.json", "My API");
});

app.Run();
```

## Step 5: Add Database Migrations

Create and apply your first migration:

```bash
# Add migration
dotnet ef migrations add Initial

# Apply migration (or use SetupDatabase in code)
dotnet ef database update
```

The `SetupDatabase<ApplicationDbContext>(options => options.MigrateOnStartup())` call in Program.cs will automatically apply migrations on app startup.

## Step 6: Run Your Application

```bash
dotnet run
```

Your API is now running with:
- ✅ Authentication endpoints at `/api/authentication/*`
- ✅ User management at `/api/authorization/users`
- ✅ Role management at `/api/authorization/roles`
- ✅ Claim management at `/api/authorization/claims`
- ✅ Tenant management at `/api/authorization/tenants`
- ✅ Default admin user (username: `admin`, password: `admin123`)

## Step 7: Test the Login Endpoint

Use curl, Postman, or your browser's Swagger UI to test:

```bash
# Login
curl -X POST http://localhost:5000/api/authentication/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "admin123",
    "tenant": null
  }'
```

You'll receive a JWT token in the response:

```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "..."
}
```

## Step 8: Protect Your Endpoints

Use the JWT token to access protected endpoints:

```csharp
app.MapGet("/api/hello", () => "Hello, authenticated user!")
    .RequireAuthorization()
    .Produces<string>();
```

Call it with the Bearer token:

```bash
curl -X GET http://localhost:5000/api/hello \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

## Next Steps

Now that your app is running:

1. **[Learn about authentication options](identity-authentication.md)** - JWT, Windows Auth, API Keys
2. **[Manage users, roles, and claims](identity-management.md)** - Built-in identity features
3. **[Enable multi-tenancy](identity-authentication.md#multi-tenancy)** - Isolate data by tenant
4. **[Write integration tests](testing.md)** - Test your APIs with `RbkTestingServer`
5. **[Enable analyzers](analyzers.md)** - Enforce API design patterns
6. **[Configure API keys](../README-ApiKeys.md)** - Machine-to-machine authentication

## Common Configuration Options

### Enable API Key Authentication

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AddApiKeyAuthentication()  // ← Add this
);
```

### Enable Windows Authentication

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .UseLoginWithWindowsAuthentication()  // ← Add this
);
```

### Enable User Self-Registration

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AllowUserSelfRegistration()  // ← Add this
);
```

### Enable Tenant Switching

```csharp
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
    .AllowTenantSwitching()  // ← Add this
);
```

## Troubleshooting

### "No authentication scheme found" error

Make sure you call both:
- `builder.Services.AddRbkRelationalAuthentication(...)`
- `app.UseRbkRelationalAuthentication()`

### "Cannot find authentication claims" error

Ensure you call `app.SetupRbkAuthenticationClaims()` before `app.Run()`.

### Database doesn't update

Make sure `app.SetupDatabase<YourDbContext>(options => options.MigrateOnStartup())` is called before `app.Run()`.

### Swagger UI returns 401

If you enable `BasicAuthenticationHandler`, make sure to call `.AllowAnonymous()` on the OpenAPI endpoints:

```csharp
app.MapOpenApi().AllowAnonymous();
```

## Support

- **Full documentation:** [docs/README.md](README.md)
- **GitHub Issues:** Report bugs and request features
- **Examples:** See `Demo1` and `Demo2` projects in the repository
