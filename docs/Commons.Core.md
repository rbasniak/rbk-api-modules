# rbkApiModules.Commons.Core

The foundation library providing core infrastructure for building ASP.NET Core Web APIs.

## Overview

`rbkApiModules.Commons.Core` provides essential building blocks for creating robust, maintainable APIs. It includes:
- Base entities for domain modeling
- Smart validation with automatic database constraint checking
- Messaging/CQRS pattern with dispatcher
- Authentication infrastructure
- File storage abstraction
- UI definition system for dynamic forms
- Application options for runtime configuration

## Key Components

### Base Entities

#### BaseEntity
The foundation entity class that all domain entities should inherit from.

```csharp
public abstract class BaseEntity
{
    public virtual Guid Id { get; protected set; }
}
```

#### TenantEntity
Entity base class for multi-tenant applications with automatic tenant filtering.

```csharp
public abstract class TenantEntity : BaseEntity
{
    public string? TenantId { get; set; }
    public bool HasTenant => !String.IsNullOrEmpty(TenantId);
    public bool HasNoTenant => String.IsNullOrEmpty(TenantId);
}
```

#### EntityReference<T>
Generic reference class for entity relationships.

```csharp
public record EntityReference<T>(T Id, string Name);
public record EntityReference(Guid Id, string Name);
```

#### EnumReference
Helper class for enum serialization and UI binding.

```csharp
public record EnumReference
{
    public int Id { get; init; }
    public string Value { get; init; }
}
```

### Authentication

#### AuthenticatedRequest
Base class for requests that require authentication context.

```csharp
public abstract class AuthenticatedRequest : IAuthenticatedRequest
{
    public AuthenticatedUser Identity { get; private set; }
    public bool IsAuthenticated { get; private set; }
    
    public void SetIdentity(string tenant, string username, string[] claims);
}
```

#### AuthenticatedUser
Represents the authenticated user with claims and tenant information.

```csharp
public class AuthenticatedUser
{
    public bool IsAuthenticated { get; }
    public bool HasTenant { get; }
    public IEnumerable<string> Claims { get; }
    public string Username { get; }
    public string? Tenant { get; }
    
    public bool HasClaim(string claim);
}
```

#### BasicAuthenticationHandler
HTTP Basic Authentication handler for API endpoints.

```csharp
public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Basic = "BasicAuthentication";
}
```

### Validation Framework

#### SmartValidator<TRequest, TModel>
Advanced validator that automatically applies database constraints to validation rules.

```csharp
public abstract class SmartValidator<TRequest, TModel> : AbstractValidator<TRequest>
    where TModel : class
{
    protected SmartValidator(DbContext context, ILocalizationService? localizationService = null)
    {
        // Automatically applies database constraints
        ApplyDatabaseConstraints();
        
        // Apply custom business rules
        ValidateBusinessRules();
    }
    
    protected abstract void ValidateBusinessRules();
}
```

**Features:**
- Automatic database constraint validation
- Tenant-aware validation for multi-tenant entities
- Localization support for validation messages
- Integration with FluentValidation

**Usage Example:**
```csharp
public class CreateUserValidator : SmartValidator<CreateUserRequest, User>
{
    public CreateUserValidator(DbContext context) : base(context)
    {
        // Database constraints are automatically applied
        // Add custom business rules
        RuleFor(x => x.Email)
            .EmailAddress()
            .MustAsync(async (email, cancellation) =>
            {
                return !await context.Set<User>()
                    .AnyAsync(u => u.Email == email, cancellation);
            })
            .WithMessage("Email already exists");
    }
}
```

### Messaging System

#### Dispatcher
Centralized message dispatcher for handling commands and queries.

```csharp
public interface IDispatcher
{
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken) 
        where TResponse : BaseResponse;
    
    Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken)
        where TNotification : INotification;
}
```

**Features:**
- Automatic request validation
- Authentication context propagation
- Logging and performance monitoring
- Error handling and response formatting

**Usage Example:**
```csharp
// In controller
public async Task<IActionResult> CreateUser(CreateUserRequest request)
{
    var response = await dispatcher.SendAsync(request, CancellationToken.None);
    return Ok(response);
}
```

### UI Definition System

#### InputControl
Dynamic UI control generation based on entity properties.

```csharp
public class InputControl
{
    public EntityReference<string> ControlType { get; set; }
    public string PropertyName { get; set; }
    public string Name { get; set; }
    public object DefaultValue { get; set; }
    public bool Required { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public List<EntityReference<object>> Data { get; set; }
}
```

**Features:**
- Automatic control type detection
- Enum value binding
- Validation rule generation
- Dependency tracking between controls

### File Storage

#### IFileStorage
Abstract file storage interface with local implementation.

```csharp
public interface IFileStorage
{
    Task<string> SaveAsync(Stream fileStream, string fileName, string contentType);
    Task<Stream> GetAsync(string filePath);
    Task DeleteAsync(string filePath);
}
```

### Email Support

#### EmailHandler
Comprehensive email sending with support for attachments and inline images.

```csharp
public static class EmailHandler
{
    public static void SendEmail(
        string smtpHost,
        bool enableSSL,
        int port,
        MailAddress sender,
        MailAddress receiver,
        string title,
        string textBody = null,
        string htmlBody = null,
        InlineImage[] inlineImages = null,
        MailAttachment[] attachments = null,
        NetworkCredential credential = null);
}
```

## Configuration

### Core Setup

```csharp
// Program.cs
builder.Services.AddRbkApiCoreSetup(options => options
    .UseDefaultCompression()
    .UseDefaultCors()
    .UseDefaultHsts(builder.Environment.IsDevelopment())
    .UseDefaultHttpsRedirection()
    .UseDefaultMemoryCache()
    .UseDefaultHttpClient()
    .UseHttpContextAccessor()
    .UseStaticFiles()
    .RegisterDbContext<ApplicationDbContext>()
);
```

### Configuration Options

All options return `RbkApiCoreOptions` for fluent chaining.

**Compression:**
- `.UseDefaultCompression()` - Gzip compression for HTTPS
- `.UseCustomCompression(Action<ResponseCompressionOptions>)` - Custom compression settings

**CORS:**
- `.UseDefaultCors()` - Allow any origin, method, header (dev-friendly)
- `.UseCustomCors(Action<CorsOptions>)` - Custom CORS policy

**HSTS (HTTP Strict Transport Security):**
- `.UseDefaultHsts(bool isDevelopment)` - HSTS with 1-year max-age (production only)
- `.UseCustomHsts(Action<HstsOptions>, bool isDevelopment)` - Custom HSTS settings

**HTTPS Redirection:**
- `.UseDefaultHttpsRedirection()` - Redirect HTTP to HTTPS (status 308, port 443)
- `.UseCustomHttpsRedirection(Action<HttpsRedirectionOptions>)` - Custom settings

**Memory Cache:**
- `.UseDefaultMemoryCache()` - Default memory cache
- `.UseCustomMemoryCache(Action<MemoryCacheOptions>)` - Custom memory cache

**HTTP Client:**
- `.UseDefaultHttpClient()` - Register `IHttpClientFactory`

**HTTP Context:**
- `.UseHttpContextAccessor()` - Register `IHttpContextAccessor`

**Static Files:**
- `.UseStaticFiles()` - Serve static files from wwwroot

**Database Context:**
- `.RegisterDbContext<TContext>()` - Register DbContext for DI (required for SmartValidator)

**Basic Authentication:**
- `.EnableBasicAuthenticationHandler()` - Enable HTTP Basic Auth (useful for Swagger UI)

## Dependencies

- Microsoft.AspNetCore.App (Framework Reference)
- Microsoft.AspNetCore.Mvc.ApiExplorer
- Microsoft.AspNetCore.OpenApi
- NSwag.SwaggerGeneration.WebApi
- FluentValidation
- Microsoft.EntityFrameworkCore
- Microsoft.AspNetCore.Http.Abstractions
- Microsoft.Extensions.Logging.Abstractions
- Microsoft.EntityFrameworkCore.Relational
- SixLabors.ImageSharp
- Swashbuckle.AspNetCore.SwaggerUI

## Best Practices

1. **Entity Design**: Always inherit from `BaseEntity` or `TenantEntity` for consistency
2. **Validation**: Use `SmartValidator` for automatic database constraint validation
3. **Authentication**: Implement `IAuthenticatedRequest` for requests requiring auth context
4. **Messaging**: Use the `Dispatcher` for all command/query handling
5. **UI Generation**: Leverage the UI definition system for dynamic form generation

## Examples

### Complete API Setup

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

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

// Add dispatcher
builder.Services.AddScoped<IDispatcher, Dispatcher>();

var app = builder.Build();

app.UseRbkApiCore();

app.Run();
```

### Controller Example

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;
    
    public UsersController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost]
    [Produces<CreateUserResponse>]
    public async Task<IActionResult> CreateUser(CreateUserRequest request)
    {
        var response = await _dispatcher.SendAsync(request, CancellationToken.None);
        return Ok(response);
    }
}
```

### Validator Example

```csharp
public class CreateUserRequest : AuthenticatedRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateUserValidator : SmartValidator<CreateUserRequest, User>
{
    public CreateUserValidator(DbContext context) : base(context)
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(50);
            
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8);
    }
}
``` 