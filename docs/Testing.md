# rbkApiModules.Commons.Testing

Comprehensive testing framework for API integration tests with built-in authentication support and fluent assertions.

## Overview

`rbkApiModules.Commons.Testing` provides a complete testing solution for ASP.NET Core Web APIs. It includes an in-memory testing server, authentication support, HTTP client wrappers, and fluent assertion methods for comprehensive integration testing.

## Key Components

### RbkTestingServer<TProgram>

Main testing server class that provides in-memory API testing capabilities.

```csharp
public abstract class RbkTestingServer<TProgram> : WebApplicationFactory<TProgram>, IAsyncInitializer 
    where TProgram : class
{
    public string InstanceId { get; }
    public Dictionary<Credentials, string> CachedCredentials { get; }
    
    // HTTP Methods
    public Task<HttpResponse> PostAsync(string url, object body, Credentials credentials);
    public Task<HttpResponse<TResponse>> PostAsync<TResponse>(string url, object body, Credentials credentials) where TResponse : class;
    public Task<HttpResponse> GetAsync(string url, Credentials credentials);
    public Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, Credentials credentials) where TResponse : class;
    public Task<HttpResponse> PutAsync(string url, object body, Credentials credentials);
    public Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, Credentials credentials) where TResponse : class;
    public Task<HttpResponse> DeleteAsync(string url, Credentials credentials);
    public Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, Credentials credentials) where TResponse : class;
    
    // Authentication
    public Task CacheCredentialsAsync(string username, string password, string? tenant);
    public Task<HttpResponse<JwtResponse>> LoginAsync(string username, string password, string? tenant);
    
    // Mock Support
    public void AddMockHttpClient<TClient, TImplementation>(IServiceCollection services, string name);
    public Mock<HttpMessageHandler> GetMockedHttpClientMessageHandler<TClient>();
}
```

**Features:**
- In-memory testing server with full ASP.NET Core pipeline
- Automatic credential caching and management
- Support for multiple authentication types (JWT, API Key, Basic Auth)
- HTTP client mocking capabilities
- SQLite in-memory database for testing
- Automatic test isolation

### HttpResponse<T>

Strongly-typed HTTP response wrapper with fluent assertion support.

```csharp
public class HttpResponse<T> : HttpResponse where T : class
{
    public T? Data { get; set; }
}

public class HttpResponse
{
    public HttpStatusCode Code { get; set; }
    public string[] Messages { get; set; }
    public string Body { get; set; }
    public ProblemDetails? Problem { get; set; }
    public bool IsSuccess => Code == HttpStatusCode.OK || Code == HttpStatusCode.NoContent;
}
```

### Authentication Support

#### Credentials
Support for different authentication types.

```csharp
public record Credentials(string Username, string Password, string? Tenant);
public record JwtToken(string Value);
public record ApiKey(string Value);
```

#### Authentication Methods
Built-in authentication support for testing.

```csharp
// Login and cache credentials
await CacheCredentialsAsync("admin", "password", "default");

// Use cached credentials
var response = await PostAsync<UserDetails>("/api/users", request, "admin");

// Direct JWT token usage
var response = await PostAsync<UserDetails>("/api/users", request, new JwtToken("token"));

// API key authentication
var response = await PostAsync<UserDetails>("/api/users", request, new ApiKey("key"));
```

## Usage Examples

### Basic Test Setup

```csharp
public class UserControllerTests : RbkTestingServer<Program>
{
    [Test]
    public async Task CreateUser_ShouldReturnSuccess()
    {
        // Arrange
        await CacheCredentialsAsync("admin", "password", "default");
        
        var request = new CreateUserRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };
        
        // Act
        var response = await PostAsync<CreateUserResponse>("/api/users", request, "admin");
        
        // Assert
        response.ShouldBeSuccess(out var user);
        user.Username.ShouldBe("testuser");
        user.Email.ShouldBe("test@example.com");
    }
}
```

### Authentication Testing

```csharp
[Test]
public async Task UnauthorizedAccess_ShouldReturnForbidden()
{
    // Act - No authentication
    var response = await PostAsync<CreateUserResponse>("/api/users", request);
    
    // Assert
    response.ShouldBeForbidden();
}

[Test]
public async Task AdminAccess_ShouldSucceed()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    // Act
    var response = await GetAsync<UserDetails[]>("/api/users", "admin");
    
    // Assert
    response.ShouldBeSuccess(out var users);
    users.ShouldNotBeEmpty();
}
```

### Error Response Testing

```csharp
[Test]
public async Task InvalidRequest_ShouldReturnValidationErrors()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    var invalidRequest = new CreateUserRequest
    {
        Username = "", // Invalid
        Email = "invalid-email", // Invalid
        Password = "123" // Too short
    };
    
    // Act
    var response = await PostAsync<CreateUserResponse>("/api/users", invalidRequest, "admin");
    
    // Assert
    response.IsSuccess.ShouldBeFalse();
    response.Code.ShouldBe(HttpStatusCode.BadRequest);
    response.Messages.ShouldContain("Username is required");
    response.Messages.ShouldContain("Email is not valid");
    response.Messages.ShouldContain("Password must be at least 8 characters");
}
```

### Mock HTTP Client Testing

```csharp
[Test]
public async Task ExternalServiceCall_ShouldUseMockedClient()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    var mockHandler = GetMockedHttpClientMessageHandler<IExternalServiceClient>();
    mockHandler.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent("{\"result\":\"success\"}")
        });
    
    // Act
    var response = await PostAsync<ExternalServiceResponse>("/api/external", request, "admin");
    
    // Assert
    response.ShouldBeSuccess();
    mockHandler.Verify(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()), Times.Once);
}
```

### Database Testing

```csharp
[Test]
public async Task CreateUser_ShouldPersistToDatabase()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    var request = new CreateUserRequest
    {
        Username = "newuser",
        Email = "newuser@example.com",
        Password = "password123"
    };
    
    // Act
    var createResponse = await PostAsync<CreateUserResponse>("/api/users", request, "admin");
    createResponse.ShouldBeSuccess(out var createdUser);
    
    // Verify persistence
    var getResponse = await GetAsync<UserDetails>($"/api/users/{createdUser.Id}", "admin");
    getResponse.ShouldBeSuccess(out var retrievedUser);
    
    // Assert
    retrievedUser.Username.ShouldBe("newuser");
    retrievedUser.Email.ShouldBe("newuser@example.com");
}
```

## Fluent Assertions

### Response Assertions

```csharp
// Success assertions
response.ShouldBeSuccess(out var data);
response.ShouldBeSuccess();

// Error assertions
response.ShouldBeForbidden();
response.ShouldBeBadRequest();
response.ShouldBeNotFound();

// Status code assertions
response.Code.ShouldBe(HttpStatusCode.OK);
response.Code.ShouldBe(HttpStatusCode.Created);

// Message assertions
response.Messages.ShouldContain("User created successfully");
response.Messages.ShouldNotContain("Error");
```

### Data Assertions

```csharp
// Type assertions
data.ShouldBeOfType<UserDetails>();
data.ShouldNotBeNull();

// Property assertions
user.Username.ShouldBe("testuser");
user.Email.ShouldBe("test@example.com");
user.IsActive.ShouldBeTrue();

// Collection assertions
users.ShouldNotBeEmpty();
users.Count.ShouldBe(5);
users.ShouldContain(u => u.Username == "admin");
```

## Configuration

### Test Server Setup

```csharp
public class UserControllerTests : RbkTestingServer<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Configure test-specific services
        builder.ConfigureServices(services =>
        {
            // Replace real services with mocks
            services.AddScoped<IEmailService, MockEmailService>();
            
            // Configure test database
            services.AddDbContext<TestDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}
```

### Authentication Configuration

```csharp
[Test]
public async Task WindowsAuthentication_ShouldWork()
{
    // Windows authentication is automatically mocked in testing environment
    var response = await PostAsync<UserDetails>("/api/users", request);
    response.ShouldBeSuccess();
}
```

### Custom HTTP Client Configuration

```csharp
[Test]
public async Task CustomHttpClient_ShouldBeMocked()
{
    // Add mocked HTTP client
    AddMockHttpClient<IExternalApiClient, ExternalApiClient>(services, "ExternalApi");
    
    var mockHandler = GetMockedHttpClientMessageHandler<IExternalApiClient>();
    mockHandler.Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK });
    
    var response = await PostAsync<ApiResponse>("/api/external", request, "admin");
    response.ShouldBeSuccess();
}
```

## Best Practices

### Test Organization

1. **Arrange-Act-Assert**: Follow the AAA pattern for clear test structure
2. **Test Isolation**: Each test should be independent and not rely on other tests
3. **Meaningful Names**: Use descriptive test names that explain the scenario
4. **Single Responsibility**: Each test should verify one specific behavior

### Authentication Testing

1. **Cache Credentials**: Use `CacheCredentialsAsync` to avoid repeated login calls
2. **Test Authorization**: Verify both authorized and unauthorized access
3. **Role Testing**: Test different user roles and permissions
4. **Token Validation**: Test JWT token expiration and refresh scenarios

### Database Testing

1. **In-Memory Database**: Use SQLite in-memory for fast, isolated tests
2. **Data Cleanup**: Ensure tests don't leave data that affects other tests
3. **Transaction Rollback**: Use transactions to isolate test data
4. **Seed Data**: Use consistent seed data for predictable test results

### Mock Usage

1. **Minimal Mocking**: Only mock external dependencies, not internal services
2. **Realistic Responses**: Mock realistic HTTP responses
3. **Verification**: Verify that mocked services are called as expected
4. **Isolation**: Ensure mocks don't interfere between tests

## Advanced Examples

### Complex Integration Test

```csharp
[Test]
public async Task CompleteUserWorkflow_ShouldSucceed()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    // Create user
    var createRequest = new CreateUserRequest
    {
        Username = "workflowuser",
        Email = "workflow@example.com",
        Password = "password123"
    };
    
    var createResponse = await PostAsync<CreateUserResponse>("/api/users", createRequest, "admin");
    createResponse.ShouldBeSuccess(out var createdUser);
    
    // Assign role
    var roleRequest = new AssignRoleRequest
    {
        UserId = createdUser.Id,
        RoleId = adminRoleId
    };
    
    var roleResponse = await PostAsync<AssignRoleResponse>("/api/users/roles", roleRequest, "admin");
    roleResponse.ShouldBeSuccess();
    
    // Verify user has role
    var userResponse = await GetAsync<UserDetails>($"/api/users/{createdUser.Id}", "admin");
    userResponse.ShouldBeSuccess(out var user);
    user.Roles.ShouldContain(r => r.Name == "Admin");
    
    // Login as new user
    await CacheCredentialsAsync("workflowuser", "password123", "default");
    
    // Verify user can access protected resources
    var protectedResponse = await GetAsync<ProtectedResource[]>("/api/protected", "workflowuser");
    protectedResponse.ShouldBeSuccess();
}
```

### Performance Testing

```csharp
[Test]
public async Task BulkUserCreation_ShouldCompleteWithinTimeout()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    var users = Enumerable.Range(1, 100).Select(i => new CreateUserRequest
    {
        Username = $"user{i}",
        Email = $"user{i}@example.com",
        Password = "password123"
    }).ToList();
    
    // Act & Assert
    var stopwatch = Stopwatch.StartNew();
    
    foreach (var user in users)
    {
        var response = await PostAsync<CreateUserResponse>("/api/users", user, "admin");
        response.ShouldBeSuccess();
    }
    
    stopwatch.Stop();
    stopwatch.ElapsedMilliseconds.ShouldBeLessThan(5000); // 5 seconds
}
```

### Error Handling Test

```csharp
[Test]
public async Task ConcurrentUserCreation_ShouldHandleConflicts()
{
    // Arrange
    await CacheCredentialsAsync("admin", "password", "default");
    
    var request = new CreateUserRequest
    {
        Username = "concurrentuser",
        Email = "concurrent@example.com",
        Password = "password123"
    };
    
    // Act - Create same user twice concurrently
    var tasks = Enumerable.Range(1, 2).Select(_ => 
        PostAsync<CreateUserResponse>("/api/users", request, "admin"));
    
    var responses = await Task.WhenAll(tasks);
    
    // Assert - One should succeed, one should fail
    var successCount = responses.Count(r => r.IsSuccess);
    var failureCount = responses.Count(r => !r.IsSuccess);
    
    successCount.ShouldBe(1);
    failureCount.ShouldBe(1);
    
    // Verify the failure has appropriate error message
    var failure = responses.First(r => !r.IsSuccess);
    failure.Messages.ShouldContain("Username already exists");
}
```

## Dependencies

- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.EntityFrameworkCore.Sqlite
- Moq
- Shouldly
- TUnit
- MimeKit

## Troubleshooting

### Common Issues

1. **Test Isolation**: Ensure tests don't share state
2. **Database Cleanup**: Use transactions or cleanup methods
3. **Authentication**: Verify credentials are cached before use
4. **Mock Configuration**: Ensure mocks are properly configured
5. **Async/Await**: Always use async/await in test methods

### Debug Tips

1. **Response Inspection**: Use `response.Body` to inspect raw responses
2. **Logging**: Enable logging to see what's happening in tests
3. **Breakpoints**: Set breakpoints in test methods for debugging
4. **Database Inspection**: Use `GetDbContext()` to inspect database state 