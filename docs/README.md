# rbkApiModules Documentation

Welcome to the comprehensive documentation for rbkApiModules. This documentation covers all aspects of the libraries, from basic setup to advanced usage patterns.

## 📚 Documentation Index

### Core Libraries

- **[Commons.Core](Commons.Core.md)** - Foundation library with base entities, validation, messaging, and common utilities
- **[Identity.Core](Identity.Core.md)** - JWT authentication, user management, and authorization system
- **[Identity.Relational](Identity.Relational.md)** - Entity Framework Core integration for identity management
- **[Commons.Testing](Testing.md)** - Comprehensive testing framework for API integration tests
- **[Analyzers](Analyzers.md)** - Static code analyzers for API design patterns and security

### Quick Start Guides

- **[Getting Started](../README.md#quick-start)** - Basic setup and configuration
- **[Installation Guide](../README.md#installation)** - Package installation instructions
- **[Configuration Examples](../README.md#configuration)** - Common configuration patterns

### Advanced Topics

- **[Multi-Tenant Support](../README.md#multi-tenant-support)** - Building multi-tenant applications
- **[Validation Framework](../README.md#validation-framework)** - Advanced validation patterns
- **[Testing Best Practices](Testing.md#best-practices)** - Comprehensive testing strategies
- **[Code Quality](Analyzers.md#best-practices)** - Maintaining code quality with analyzers

## 🚀 Getting Started

### 1. Choose Your Packages

Start with the core packages you need:

```bash
# Core infrastructure
dotnet add package rbkApiModules.Commons.Core

# Identity management
dotnet add package rbkApiModules.Identity.Core
dotnet add package rbkApiModules.Identity.Relational

# Testing framework
dotnet add package rbkApiModules.Commons.Testing

# Code analyzers
dotnet add package rbkApiModules.Analyzers
```

### 2. Basic Setup

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add core services
builder.Services.AddRbkApiCore(options =>
{
    options.UseDefaultSwagger()
           .UseDefaultHsts()
           .RegisterAdditionalValidators(typeof(Program).Assembly);
});

// Add identity services
builder.Services.AddRbkRelationalAuthentication(options =>
{
    options.UseJwtAuthentication()
           .AllowTenantSwitching();
});

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

var app = builder.Build();

app.UseRbkApiCore();
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

### 3. Configuration

```json
// appsettings.json
{
  "ConnectionStrings": {
    "Default": "Server=(localdb)\\mssqllocaldb;Database=YourApp;Trusted_Connection=true"
  },
  "JwtIssuerOptions": {
    "Issuer": "YourApplication",
    "Audience": "YourApplication",
    "SecretKey": "your-super-secret-key-with-at-least-32-characters",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

## 📖 Detailed Documentation

### Commons.Core
The foundation library providing essential building blocks for ASP.NET Core Web APIs.

**Key Features:**
- Base entities (`BaseEntity`, `TenantEntity`)
- Authentication infrastructure (`AuthenticatedRequest`, `BasicAuthenticationHandler`)
- Advanced validation framework (`SmartValidator`)
- Messaging system (`Dispatcher`)
- UI definition system for dynamic form generation
- File storage and email support

**[Read Full Documentation →](Commons.Core.md)**

### Identity.Core
Comprehensive identity and authentication management with JWT support.

**Key Features:**
- JWT token generation and validation
- User management and role-based authorization
- Multi-tenant authentication support
- Windows authentication integration
- API key authentication
- Custom claims system
- Built-in email templates

**[Read Full Documentation →](Identity.Core.md)**

### Identity.Relational
Entity Framework Core integration for the identity system.

**Key Features:**
- Complete EF Core configuration for identity entities
- Automatic tenant filtering and relationship management
- User-role many-to-many relationships
- Claims-based authorization
- Default admin user setup

**[Read Full Documentation →](Identity.Relational.md)**

### Commons.Testing
Comprehensive testing framework for API integration tests.

**Key Features:**
- In-memory testing server (`RbkTestingServer`)
- Built-in authentication support
- HTTP client wrappers with fluent assertions
- Mock HTTP client capabilities
- SQLite in-memory database for testing

**[Read Full Documentation →](Testing.md)**

### Analyzers
Static code analyzers for enforcing API design patterns and security.

**Key Features:**
- Endpoint authorization analyzer (RBK201)
- Endpoint produces analyzer (RBK101-RBK105)
- Automatic code fixes
- Swagger/OpenAPI integration
- Security-focused analyzers

**[Read Full Documentation →](Analyzers.md)**

## 🔧 Common Patterns

### Multi-Tenant Applications

```csharp
// Entity with tenant support
public class User : TenantEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
}

// Tenant-aware validation
public class CreateUserValidator : SmartValidator<CreateUserRequest, User>
{
    public CreateUserValidator(DbContext context) : base(context)
    {
        // Database constraints and tenant filtering are automatic
    }
}
```

### Command/Query Pattern

```csharp
// Command
public class CreateUserRequest : AuthenticatedRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
}

// Handler
public class CreateUserHandler : IRequestHandler<CreateUserRequest, CreateUserResponse>
{
    public async Task<CreateUserResponse> HandleAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        // Implementation
        return new CreateUserResponse { Id = user.Id };
    }
}

// Controller
[HttpPost]
[Produces<CreateUserResponse>]
public async Task<IActionResult> CreateUser(CreateUserRequest request)
{
    var response = await dispatcher.SendAsync(request, CancellationToken.None);
    return Ok(response);
}
```

### Testing Patterns

```csharp
public class UserControllerTests : RbkTestingServer<Program>
{
    [Test]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        // Arrange
        await CacheCredentialsAsync("admin", "password", "default");
        
        // Act
        var response = await PostAsync<CreateUserResponse>("/api/users", request, "admin");
        
        // Assert
        response.ShouldBeSuccess(out var user);
        user.Username.ShouldBe("testuser");
    }
}
```

## 🛠️ Troubleshooting

### Common Issues

1. **Authentication Problems**
   - Verify JWT configuration in `appsettings.json`
   - Check that authentication middleware is configured correctly
   - Ensure endpoints have proper authorization attributes

2. **Validation Errors**
   - Check that validators are registered in DI container
   - Verify database constraints match validation rules
   - Ensure `SmartValidator` is used for automatic constraint validation

3. **Testing Issues**
   - Ensure test server is properly configured
   - Check that credentials are cached before making requests
   - Verify database is seeded with test data

4. **Analyzer Warnings**
   - Add missing `Produces<T>()` declarations
   - Add `RequireAuthorization()` or `AllowAnonymous()` to endpoints
   - Use suppression attributes for legitimate exceptions

### Getting Help

- **GitHub Issues**: Report bugs and request features
- **Documentation**: Check the detailed documentation for each package
- **Examples**: Review the example projects in the repository
- **Community**: Join discussions in the community forums

## 📝 Contributing

We welcome contributions! Please see our contributing guidelines:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

---

**Need help?** Check our [main README](../README.md) for quick start information or browse the detailed documentation above. 





------------------

// TODO: outbox explanation

sequenceDiagram
    autonumber
    participant UI as API / Command Handler
    participant UoW as DbContext / UnitOfWork
    participant AR as Aggregate Root (Project)
    participant OI as Outbox Interceptor
    participant DB as Database (Outbox)
    participant DIS as Outbox Dispatcher (Background Service)
    participant REG as EventTypeRegistry
    participant IB as Inbox (Idempotency)
    participant HND as Event Handler(s)

    UI->>UoW: Load Project (by id)
    UoW-->>UI: Project instance
    UI->>AR: ConsumeMaterial(materialId, qty, unit)
    activate AR
    AR->>AR: Enforce invariants & mutate state
    AR->>AR: RaiseDomainEvent(ProjectMaterialAdded)
    deactivate AR

    UI->>UoW: SaveChanges()
    activate UoW
    UoW->>OI: SavingChanges (interceptor hook)
    activate OI
    OI->>OI: Wrap event in Envelope<T>\n(+ TenantId, CorrelationId, etc.)
    OI->>DB: INSERT OutboxMessages(payload = serialized envelope)
    deactivate OI
    UoW->>DB: Commit transaction (domain + outbox)
    deactivate UoW
    Note over UI,DB: Domain change and outbox row are committed atomically

    loop Poll every N ms
        DIS->>DB: SELECT unprocessed Outbox messages (FIFO, batch)
        alt Found messages
            DIS->>DIS: Deserialize JSON → Envelope<T>
            DIS->>REG: Resolve Name+Version → CLR Type
            REG-->>DIS: CLR Type

            %% Idempotency per handler
            DIS->>IB: Check (EventId, HandlerName)
            alt Already processed
                IB-->>DIS: Exists → skip handler
            else Not processed
                IB-->>DIS: Not found
                DIS->>HND: Handle(envelope)
                activate HND
                HND-->>DIS: OK (side effects / projections)
                deactivate HND
                DIS->>IB: INSERT (EventId, HandlerName, ProcessedUtc)
            end

            DIS->>DB: Mark Outbox.ProcessedUtc
        else None
            DIS-->>DIS: Sleep until next poll
        end
    end
  
