# rbkApiModules.Analyzers

Static code analyzers for enforcing API design patterns, security best practices, and Swagger/OpenAPI compliance.

## Overview

`rbkApiModules.Analyzers` provides Roslyn-based static analyzers that help maintain consistent API design patterns and enforce security best practices. The analyzers integrate with Visual Studio and other IDEs to provide real-time feedback during development.

## Key Analyzers

### EndpointAuthorizationAnalyzer

Ensures all API endpoints explicitly declare their authorization policy.

**Diagnostic ID:** RBK201

**Purpose:** Enforces security best practices by requiring explicit authorization declarations on all endpoints.

**Rules:**
- All endpoints must declare either `AllowAnonymous()` or `RequireAuthorization()`
- Prevents accidental exposure of endpoints without proper authorization

**Example:**

```csharp
// ❌ This will trigger RBK201
app.MapPost("/api/users", CreateUser)
    .Produces<CreateUserResponse>();

// ✅ This is correct
app.MapPost("/api/users", CreateUser)
    .Produces<CreateUserResponse>()
    .RequireAuthorization();
```

### EndpointProducesAnalyzer

Validates correct return type declarations for API endpoints.

**Diagnostic IDs:**
- RBK101: Missing `Produces<T>()` on endpoint
- RBK102: Wrong `Produces<T>()` type declaration
- RBK103: Missing `Produces()` for void endpoints
- RBK104: `Produces()` used with return value
- RBK105: Handler returns multiple types

**Purpose:** Ensures accurate Swagger/OpenAPI documentation by validating return type declarations match actual handler implementations.

**Examples:**

```csharp
// ❌ RBK101: Missing Produces<T>()
app.MapGet("/api/users", GetUsers);

// ✅ Correct
app.MapGet("/api/users", GetUsers)
    .Produces<UserDetails[]>();

// ❌ RBK102: Wrong type
app.MapGet("/api/users", GetUsers)
    .Produces<string>(); // Handler returns UserDetails[]

// ✅ Correct
app.MapGet("/api/users", GetUsers)
    .Produces<UserDetails[]>();

// ❌ RBK103: Missing Produces() for void
app.MapPost("/api/logout", Logout);

// ✅ Correct
app.MapPost("/api/logout", Logout)
    .Produces();

// ❌ RBK104: Produces() with return value
app.MapGet("/api/users", GetUsers)
    .Produces(); // Handler returns UserDetails[]

// ✅ Correct
app.MapGet("/api/users", GetUsers)
    .Produces<UserDetails[]>();
```

## Installation

### NuGet Package

```xml
<PackageReference Include="rbkApiModules.Analyzers" Version="1.0.0" />
```

### Visual Studio

The analyzers will automatically run in Visual Studio and provide warnings/errors in the Error List window.

### Command Line

Analyzers run automatically during compilation:

```bash
dotnet build
```

## Configuration

### .editorconfig

Configure analyzer behavior using `.editorconfig`:

```ini
[*.cs]
# Enable all RBK analyzers
dotnet_diagnostic.RBK101.severity = error
dotnet_diagnostic.RBK102.severity = error
dotnet_diagnostic.RBK103.severity = error
dotnet_diagnostic.RBK104.severity = error
dotnet_diagnostic.RBK105.severity = error
dotnet_diagnostic.RBK201.severity = error

# Or disable specific analyzers
dotnet_diagnostic.RBK201.severity = none
```

### Global Suppression

Suppress specific diagnostics globally:

```csharp
// GlobalSuppressions.cs
[assembly: SuppressMessage("Security", "RBK201:Missing AllowAnonymous or RequireAuthorization on endpoint", Justification = "Legacy code")]
```

### Local Suppression

Suppress diagnostics for specific code:

```csharp
#pragma warning disable RBK201
app.MapGet("/api/health", HealthCheck);
#pragma warning restore RBK201
```

## Code Fixes

### Automatic Fixes

The analyzers provide automatic code fixes for common issues:

1. **RBK101**: Add `Produces<T>()` based on handler return type
2. **RBK103**: Add `Produces()` for void endpoints
3. **RBK201**: Add `RequireAuthorization()` or `AllowAnonymous()`

### Using Code Fixes

1. **Visual Studio**: Click the lightbulb icon next to the diagnostic
2. **Command Line**: Use `dotnet format` with analyzer fixes
3. **IDE**: Use the quick fix suggestions in your IDE

## Best Practices

### Authorization Patterns

```csharp
// Public endpoints
app.MapGet("/api/health", HealthCheck)
    .AllowAnonymous();

// Authenticated endpoints
app.MapGet("/api/users", GetUsers)
    .RequireAuthorization();

// Role-based authorization
app.MapPost("/api/admin/users", CreateUser)
    .RequireAuthorization(policy => policy.RequireRole("Admin"));

// Policy-based authorization
app.MapDelete("/api/users/{id}", DeleteUser)
    .RequireAuthorization("UserManagementPolicy");
```

### Return Type Declarations

```csharp
// Simple return types
app.MapGet("/api/users", GetUsers)
    .Produces<UserDetails[]>();

// Multiple return types
app.MapGet("/api/users/{id}", GetUser)
    .Produces<UserDetails>()
    .ProducesProblem(404);

// Void endpoints
app.MapPost("/api/logout", Logout)
    .Produces();

// File downloads
app.MapGet("/api/files/{id}", DownloadFile)
    .Produces<FileStreamResult>();
```

### Handler Implementation

```csharp
public class UserHandler
{
    public async Task<HandlerResult<UserDetails[]>> HandleAsync(GetUsersRequest request)
    {
        // Ensure consistent return types
        return HandlerResult<UserDetails[]>.Success(users);
    }
    
    public async Task<HandlerResult> HandleAsync(LogoutRequest request)
    {
        // Void operations
        return HandlerResult.Success();
    }
}
```

## Integration with Swagger/OpenAPI

### Accurate Documentation

The analyzers ensure your Swagger documentation accurately reflects your API:

```csharp
// This will generate correct OpenAPI documentation
app.MapGet("/api/users", GetUsers)
    .Produces<UserDetails[]>(200, "application/json")
    .ProducesProblem(400)
    .ProducesProblem(401)
    .ProducesProblem(403)
    .WithOpenApi(operation =>
    {
        operation.Summary = "Get all users";
        operation.Description = "Retrieves a list of all users";
        return operation;
    });
```

### Security Documentation

Authorization requirements are automatically documented:

```csharp
app.MapPost("/api/users", CreateUser)
    .RequireAuthorization()
    .WithOpenApi(operation =>
    {
        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    new string[] {}
                }
            }
        };
        return operation;
    });
```

## Advanced Configuration

### Custom Analyzer Rules

Extend the analyzers with custom rules:

```csharp
// Custom analyzer for specific patterns
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CustomEndpointAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "RBK301";
    
    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Custom endpoint rule",
        "Custom endpoint validation",
        "Custom",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);
    
    public override void Initialize(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeEndpoint, SyntaxKind.InvocationExpression);
    }
    
    private void AnalyzeEndpoint(SyntaxNodeAnalysisContext context)
    {
        // Custom analysis logic
    }
}
```

### Suppression Rules

Configure when to suppress specific diagnostics:

```csharp
// Suppress for specific file patterns
[assembly: SuppressMessage("Security", "RBK201", 
    Justification = "Health check endpoints are intentionally public",
    Scope = "member",
    Target = "~M:HealthController.Get()")]

// Suppress for specific conditions
[assembly: SuppressMessage("Swagger", "RBK101", 
    Justification = "Legacy API endpoints",
    Scope = "namespace",
    Target = "~N:LegacyApi")]
```

## Troubleshooting

### Common Issues

1. **False Positives**: Use suppression attributes for legitimate cases
2. **Performance**: Analyzers run during compilation, minimal performance impact
3. **IDE Integration**: Ensure your IDE supports Roslyn analyzers
4. **Build Failures**: Fix all analyzer errors before deployment

### Debugging Analyzers

Enable detailed logging for analyzer debugging:

```xml
<PropertyGroup>
  <EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>
  <RunAnalyzersDuringBuild>true</RunAnalyzersDuringBuild>
  <RunAnalyzersDuringLiveAnalysis>true</RunAnalyzersDuringLiveAnalysis>
</PropertyGroup>
```

### Custom Rules

Create custom analyzer rules for your specific requirements:

```csharp
public class CustomEndpointAnalyzer : DiagnosticAnalyzer
{
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeEndpoint, SyntaxKind.InvocationExpression);
    }
    
    private void AnalyzeEndpoint(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        
        // Custom analysis logic
        if (IsEndpointCall(invocation))
        {
            // Check for custom rules
            if (!HasRequiredCustomAttribute(invocation))
            {
                var diagnostic = Diagnostic.Create(CustomRule, invocation.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
```

## Migration Guide

### From Manual Validation

If you're migrating from manual validation to automated analyzers:

1. **Install Package**: Add the analyzer package to your project
2. **Fix Existing Issues**: Address all existing analyzer warnings
3. **Configure Rules**: Set up `.editorconfig` for your team
4. **CI Integration**: Ensure analyzers run in your CI/CD pipeline

### Gradual Adoption

For large codebases, adopt analyzers gradually:

```ini
# Start with warnings, then move to errors
dotnet_diagnostic.RBK201.severity = warning
dotnet_diagnostic.RBK101.severity = warning

# Later, change to errors
dotnet_diagnostic.RBK201.severity = error
dotnet_diagnostic.RBK101.severity = error
```

## CI/CD Integration

### GitHub Actions

```yaml
name: Code Analysis
on: [push, pull_request]
jobs:
  analyze:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v3
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '9.0.x'
    - name: Build and Analyze
      run: |
        dotnet build --verbosity normal
        dotnet format --verify-no-changes
```

### Azure DevOps

```yaml
- task: DotNetCoreCLI@2
  displayName: 'Build and Analyze'
  inputs:
    command: 'build'
    arguments: '--verbosity normal'
    projects: '**/*.csproj'
```

## Performance Considerations

### Analyzer Performance

- Analyzers run during compilation and IDE analysis
- Minimal performance impact on build times
- Concurrent execution enabled for better performance
- Generated code analysis disabled by default

### Optimization Tips

1. **Selective Analysis**: Only enable analyzers you need
2. **Suppression**: Use suppression for false positives
3. **Configuration**: Optimize analyzer settings for your codebase
4. **CI Integration**: Run analyzers in CI/CD for faster feedback

## Future Enhancements

### Planned Features

1. **More Analyzers**: Additional analyzers for common patterns
2. **Custom Rules**: Framework for creating custom analyzers
3. **IDE Integration**: Enhanced IDE support and quick fixes
4. **Performance**: Further performance optimizations

### Contributing

To contribute new analyzers or improvements:

1. Fork the repository
2. Create a feature branch
3. Implement your analyzer
4. Add tests
5. Submit a pull request

## Support

For issues, questions, or contributions:

1. **GitHub Issues**: Report bugs and request features
2. **Documentation**: Check the documentation for usage examples
3. **Community**: Join discussions in the community forums
4. **Contributing**: Submit pull requests for improvements 