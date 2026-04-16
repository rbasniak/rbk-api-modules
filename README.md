# rbkApiModules

A comprehensive suite of .NET libraries for building production-ready ASP.NET Core Web APIs with authentication, validation, testing, and code analysis.

## What is rbkApiModules?

rbkApiModules accelerates Web API development by providing battle-tested infrastructure for common patterns: JWT authentication, multi-tenancy, role-based authorization, validation, messaging, and testing. Build secure, maintainable APIs faster.

## Packages

| Package | Purpose |
|---------|---------|
| **rbkApiModules.Commons.Core** | Core infrastructure: base entities, validation, messaging, file storage, UI definitions |
| **rbkApiModules.Identity.Core** | Authentication & authorization: JWT, API keys, Windows Auth, users, roles, claims |
| **rbkApiModules.Commons.Testing** | Integration testing: in-memory server, fluent assertions, mock support |
| **rbkApiModules.Analysers** | Code analyzers: enforce authorization, validate Swagger annotations |

## Quick Install

```bash
dotnet add package rbkApiModules.Commons.Core
dotnet add package rbkApiModules.Identity.Core
dotnet add package rbkApiModules.Commons.Testing    # For test projects
dotnet add package rbkApiModules.Analysers           # Optional
```

## Quick Start

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite(connectionString));

// Add rbk core services
builder.Services.AddRbkApiCoreSetup(options => options
    .UseDefaultCompression()
    .UseDefaultCors()
    .RegisterDbContext<AppDbContext>()
);

// Add authentication
builder.Services.AddRbkRelationalAuthentication(options => options
    .UseSymetricEncryptationKey()
);

var app = builder.Build();

// Use middleware
app.UseRbkApiCoreSetup();
app.UseRbkRelationalAuthentication();

// Setup database and claims
app.SetupDatabase<AppDbContext>(options => options.MigrateOnStartup());
app.SetupRbkAuthenticationClaims();
app.SetupRbkDefaultAdmin(options => options
    .WithUsername("admin")
    .WithPassword("admin123")
    .WithEmail("admin@example.com")
);

app.Run();
```

That's it! Your API now has JWT authentication, user management, role-based authorization, and built-in endpoints for users/roles/claims.

## Key Features

✅ **JWT Authentication** - Token generation, refresh tokens, configurable expiration  
✅ **API Key Authentication** - Machine-to-machine auth with rate limiting ([docs](README-ApiKeys.md))  
✅ **Multi-Tenancy** - Automatic tenant filtering and isolation  
✅ **Role-Based Authorization** - Users, roles, claims with fine-grained control  
✅ **Smart Validation** - Auto-apply database constraints to FluentValidation rules  
✅ **Messaging/CQRS** - Dispatcher with request handlers and validation pipeline  
✅ **Testing Framework** - In-memory test server with authentication and assertions  
✅ **Code Analyzers** - Enforce authorization and Swagger annotations at compile-time  
✅ **Built-in Endpoints** - User/role/claim/tenant management out of the box  

## Documentation

📖 **[Getting Started](docs/getting-started.md)** - 10-minute setup guide

**Core Features:**
- **[Commons.Core](docs/commons-core.md)** - Base entities, validation, messaging, file storage
- **[Identity & Authentication](docs/identity-authentication.md)** - JWT, API keys, Windows Auth setup
- **[Identity Management](docs/identity-management.md)** - Users, roles, claims, tenants
- **[API Keys](README-ApiKeys.md)** - Machine-to-machine authentication with rate limiting
- **[Testing Framework](docs/Testing.md)** - Write integration tests with `RbkTestingServer`
- **[Code Analyzers](docs/Analyzers.md)** - Static analysis for security and Swagger

**Advanced:**
- **[UI Definitions](docs/ui-definitions.md)** - Dynamic form generation from entity metadata
- **[Application Options](docs/application-options.md)** - Runtime-editable configuration
- **[Database Seeding](docs/seeding.md)** - Migrations, seeds, and deferred seeding
- **[Built-in Endpoints](docs/built-in-endpoints.md)** - Complete API reference for identity endpoints

**Complete Index:** **[docs/README.md](docs/README.md)**

## Example: Protect an Endpoint

```csharp
// JWT or API key with specific claim required
app.MapGet("/api/data", GetData)
    .RequireAuthorization()
    .RequireAuthorizationClaim("READ_DATA")
    .Produces<DataResponse>();

// API key only
app.MapPost("/api/webhook", ProcessWebhook)
    .RequireAuthenticationApiKey("WEBHOOK_ACCESS")
    .Produces();

// Anonymous
app.MapGet("/api/health", () => "OK")
    .AllowAnonymous()
    .Produces<string>();
```

## Example: Write a Test

```csharp
public class DataControllerTests : RbkTestingServer<Program>
{
    [Test]
    public async Task GetData_ReturnsData()
    {
        // Arrange: cache credentials
        await CacheCredentialsAsync("admin", "password", tenant: null);
        
        // Act: call endpoint
        var response = await GetAsync<DataResponse>("/api/data", "admin");
        
        // Assert
        response.ShouldBeSuccess(out var data);
        data.Items.ShouldNotBeEmpty();
    }
}
```

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Support

- **Issues & Features:** [GitHub Issues](https://github.com/yourusername/rbk-api-modules/issues)
- **Examples:** See `Demo1` and `Demo2` projects in this repository
- **Documentation:** [docs/README.md](docs/README.md)