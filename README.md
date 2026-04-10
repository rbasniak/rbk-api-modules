# rbkApiModules

A comprehensive set of .NET libraries designed to accelerate the development of ASP.NET Core Web APIs with built-in authentication, validation, testing utilities, and code analysis tools.

## Overview

rbkApiModules provides a modular approach to building robust, secure, and maintainable ASP.NET Core applications. The libraries include:

- **Core Infrastructure**: Base entities, validation, messaging, and common utilities
- **Identity Management**: JWT authentication, user management, and authorization
- **Testing Framework**: Comprehensive testing utilities for API integration tests
- **Code Analysis**: Static analyzers for enforcing API design patterns
- **Database Integration**: Entity Framework Core integration with tenant support

## Packages

### rbkApiModules.Commons.Core

The foundation library providing core infrastructure for building APIs.

#### Key Features

- **Base Entities**: `BaseEntity`, `TenantEntity`, `EntityReference<T>`, `EnumReference`
- **Authentication**: `AuthenticatedRequest`, `BasicAuthenticationHandler`, `AuthenticatedUser`
- **Validation**: `SmartValidator<TRequest, TModel>` with automatic database constraint validation
- **Messaging**: `Dispatcher` for handling commands and queries with automatic validation
- **UI Definition**: Dynamic UI control generation based on entity properties
- **File Storage**: `IFileStorage` interface with local implementation
- **Email Support**: `EmailHandler` for sending emails with attachments and inline images

#### Usage Example

```csharp
// Configure services
services.AddRbkApiCore(options =>
{
    options.UseDefaultSwagger()
           .UseDefaultHsts()
           .EnableBasicAuthenticationHandler()
           .RegisterAdditionalValidators(typeof(Startup).Assembly);
});

// Use SmartValidator for automatic database constraint validation
public class CreateUserValidator : SmartValidator<CreateUserRequest, User>
{
    public CreateUserValidator(DbContext context) : base(context)
    {
        // Database constraints are automatically applied
        // Add custom business rules here
    }
}
```

### rbkApiModules.Identity.Core

Comprehensive identity and authentication management with JWT support.

#### Key Features

- **JWT Authentication**: Token generation, validation, and refresh token support
- **User Management**: User creation, authentication, and role assignment
- **Tenant Support**: Multi-tenant authentication with tenant switching
- **Windows Authentication**: Integration with Windows authentication
- **API Key Authentication**: Support for API key-based authentication
- **Custom Claims**: Extensible claim handling system
- **Email Templates**: Built-in HTML and text email templates for user confirmation

#### Usage Example

```csharp
// Configure JWT authentication
services.AddRbkAuthentication(options =>
{
    options.UseJwtAuthentication()
           .AllowTenantSwitching()
           .AllowUserCreationOnFirstAccess();
});

// Configure JWT options in appsettings.json
{
  "JwtIssuerOptions": {
    "Issuer": "YourApp",
    "Audience": "YourApp",
    "SecretKey": "your-secret-key-here",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

### rbkApiModules.Identity.Relational

Entity Framework Core integration for the Identity system.

#### Key Features

- **Database Context**: Complete EF Core configuration for identity entities
- **Tenant Support**: Automatic tenant filtering and relationship management
- **User-Role Management**: Many-to-many relationships with proper constraints
- **Claims System**: Flexible claims-based authorization
- **Default Admin Setup**: Automatic creation of default admin user and claims

#### Usage Example

```csharp
// Add relational identity services
services.AddRbkRelationalAuthentication(options =>
{
    options.UseJwtAuthentication()
           .AllowTenantSwitching();
});

// Configure database context
services.AddDbContext<YourDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// Setup default admin user
app.SetupRbkDefaultAdmin(options =>
{
    options.AdminUsername = "admin";
    options.AdminPassword = "admin123";
    options.AdminTenant = "default";
});
```

### rbkApiModules.Commons.Testing

Comprehensive testing framework for API integration tests.

#### Key Features

- **Testing Server**: `RbkTestingServer<TProgram>` for in-memory API testing
- **Authentication Support**: Built-in login and credential caching
- **HTTP Client Wrappers**: Simplified HTTP request methods with authentication
- **Response Assertions**: Fluent assertion methods for HTTP responses
- **Mock Support**: HTTP client mocking capabilities
- **Database Testing**: SQLite in-memory database for testing

#### Usage Example

```csharp
public class UserControllerTests : RbkTestingServer<Program>
{
    [Test]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        // Login and cache credentials
        await CacheCredentialsAsync("admin", "password", "default");
        
        // Make authenticated request
        var response = await PostAsync<UserDetails>("/api/users", new CreateUserRequest
        {
            Username = "testuser",
            Email = "test@example.com"
        }, "admin");
        
        // Assert response
        response.ShouldBeSuccess(out var user);
        user.Username.ShouldBe("testuser");
    }
}
```

### rbkApiModules.Analysers

Static code analyzers for enforcing API design patterns and security.

#### Key Features

- **Endpoint Authorization Analyzer**: Ensures all endpoints declare authorization policy
- **Endpoint Produces Analyzer**: Validates correct return type declarations
- **Code Fixes**: Automatic code fixes for common issues
- **Swagger Integration**: Analyzers work with Swagger/OpenAPI generation

#### Analyzers

1. **RBK101**: Missing `Produces<T>()` on endpoint
2. **RBK102**: Wrong `Produces<T>()` type declaration
3. **RBK103**: Missing `Produces()` for void endpoints
4. **RBK104**: `Produces()` used with return value
5. **RBK105**: Handler returns multiple types
6. **RBK201**: Missing `AllowAnonymous()` or `RequireAuthorization()` on endpoint

#### Usage

Add the analyzer package to your project:

```xml
<PackageReference Include="rbkApiModules.Analysers" Version="1.0.0" />
```

The analyzers will automatically run during compilation and provide warnings/errors for code that doesn't follow the established patterns.

## Installation

### Core Package
```bash
dotnet add package rbkApiModules.Commons.Core
```

### Identity Packages
```bash
dotnet add package rbkApiModules.Identity.Core
dotnet add package rbkApiModules.Identity.Relational
```

### Testing Package
```bash
dotnet add package rbkApiModules.Commons.Testing
```

### Analyzers Package
```bash
dotnet add package rbkApiModules.Analysers
```

## Quick Start

1. **Add Core Services**
```csharp
// Program.cs
builder.Services.AddRbkApiCore(options =>
{
    options.UseDefaultSwagger()
           .UseDefaultHsts();
});
```

2. **Configure Identity**
```csharp
builder.Services.AddRbkRelationalAuthentication(options =>
{
    options.UseJwtAuthentication()
           .AllowTenantSwitching();
});
```

3. **Setup Database**
```csharp
builder.Services.AddDbContext<YourDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});
```

4. **Configure Authentication**
```csharp
// appsettings.json
{
  "JwtIssuerOptions": {
    "Issuer": "YourApp",
    "Audience": "YourApp", 
    "SecretKey": "your-secret-key-here",
    "AccessTokenLife": 60,
    "RefreshTokenLife": 1440
  }
}
```

5. **Setup Default Admin**
```csharp
app.SetupRbkDefaultAdmin(options =>
{
    options.AdminUsername = "admin";
    options.AdminPassword = "admin123";
});
```

## Features

### Multi-Tenant Support
- Automatic tenant filtering for entities inheriting from `TenantEntity`
- Tenant-aware authentication and authorization
- Support for tenant switching in JWT tokens

### Validation Framework
- Automatic database constraint validation
- FluentValidation integration
- Custom business rule validation
- Localization support for validation messages

### Testing Framework
- In-memory testing server
- Automatic credential caching
- Fluent HTTP response assertions
- Mock HTTP client support

### Code Quality
- Static analyzers for API design patterns
- Automatic code fixes
- Security-focused analyzers
- Swagger/OpenAPI integration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## 📚 Documentation

For detailed documentation, examples, and advanced usage patterns, see our comprehensive documentation:

- **[📖 Full Documentation](docs/README.md)** - Complete documentation index
- **[🔧 Commons.Core](docs/Commons.Core.md)** - Core infrastructure and utilities
- **[🔐 Identity.Core](docs/Identity.Core.md)** - Authentication and authorization
- **[🗄️ Identity.Relational](docs/Identity.Relational.md)** - Database integration
- **[🧪 Testing Framework](docs/Testing.md)** - API testing utilities
- **[🔍 Code Analyzers](docs/Analyzers.md)** - Static analysis tools

## Support

For questions, issues, or contributions, please open an issue on the GitHub repository.