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
    public Task<HttpResponse> PostMultipartAsync(string url, MultipartFormDataContent body);
    public Task<HttpResponse> PostMultipartAsync(string url, MultipartFormDataContent body, ApiKey credentials);
    public Task<HttpResponse> PostMultipartAsync(string url, MultipartFormDataContent body, JwtToken credentials);
    public Task<HttpResponse> PostMultipartAsync(string url, MultipartFormDataContent body, Credentials credentials);
    public Task<HttpResponse> PostMultipartAsync(string url, MultipartFormDataContent body, string username);
    public Task<HttpResponse<TResponse>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent body) where TResponse : class;
    public Task<HttpResponse<TResponse>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent body, ApiKey credentials) where TResponse : class;
    public Task<HttpResponse<TResponse>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent body, JwtToken credentials) where TResponse : class;
    public Task<HttpResponse<TResponse>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent body, Credentials credentials) where TResponse : class;
    public Task<HttpResponse<TResponse>> PostMultipartAsync<TResponse>(string url, MultipartFormDataContent body, string username) where TResponse : class;
    public Task<HttpResponse> GetAsync(string url, Credentials credentials);
    public Task<HttpResponse<TResponse>> GetAsync<TResponse>(string url, Credentials credentials) where TResponse : class;
    public Task<HttpResponse> PutAsync(string url, object body, Credentials credentials);
    public Task<HttpResponse<TResponse>> PutAsync<TResponse>(string url, object body, Credentials credentials) where TResponse : class;
    public Task<HttpResponse> DeleteAsync(string url, Credentials credentials);
    public Task<HttpResponse<TResponse>> DeleteAsync<TResponse>(string url, Credentials credentials) where TResponse : class;
    
    // Authentication
    public Task CacheCredentialsAsync(string username, string password, string? tenant);
    public Task<HttpResponse<JwtResponse>> LoginAsync(string username, string password, string? tenant);
    
    // Outbound HTTP client registration (ConfigureTestServices)
    public void AddMockHttpClient<TClient, TImplementation>(IServiceCollection services, string? name = null);
    public void AddNamedHttpClient(IServiceCollection services, string name);
    public void SetNamedHttpClient(string name, HttpClient client);

    // Fluent outbound HTTP mocks (per test, inside HttpMockScope)
    public HttpMockScope HttpMockScope();
    public HttpMockCallBuilder MockHttpGet<TClient>(Func<string, bool>? urlMatcher = null);
    public HttpMockCallBuilder MockHttpPost<TClient>(Func<string, bool>? urlMatcher = null);
    public HttpMockCallBuilder MockHttpCall<TClient>(HttpMethod method, Func<string, bool>? urlMatcher = null);
}
```

**Features:**
- In-memory testing server with full ASP.NET Core pipeline
- Automatic credential caching and management
- Support for multiple authentication types (JWT, API Key, Basic Auth)
- Fluent outbound HTTP mocking with `AsyncLocal` test isolation
- Named HttpClient factory auto-wiring for cross-API and external stubs
- Multipart form uploads with API key authentication
- SQLite in-memory database for testing
- Automatic test isolation

### CustomHttpClientFactory

Supplies pre-registered `HttpClient` instances as the application's `IHttpClientFactory` during integration tests. You normally do **not** register this yourself — `AddMockHttpClient` and `AddNamedHttpClient` wire it automatically.

```csharp
public sealed class CustomHttpClientFactory : IHttpClientFactory
{
    public CustomHttpClientFactory(ConcurrentDictionary<string, HttpClient> clients);
    public HttpClient CreateClient(string name);
}
```

Client names must match the production `AddHttpClient(..., name)` registration (typically `nameof(TClient)`). `CreateClient` throws if the name is not registered.

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

#### Multipart uploads

Use `PostMultipartAsync` for endpoints that accept `multipart/form-data` (file uploads, mixed form fields). Overloads mirror `PostAsync`: no authentication, cached username/`Credentials`, `JwtToken`, or `ApiKey`.

```csharp
using var content = new MultipartFormDataContent();
var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync("document.pdf"));
fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
content.Add(fileContent, "file", "document.pdf");

// API key
var response = await PostMultipartAsync<UploadResponse[]>("api/capture/v1/file", content, new ApiKey("my-api-key"));

// Cached JWT (after CacheCredentialsAsync)
await CacheCredentialsAsync("admin", "password", "default");
var response = await PostMultipartAsync<UploadResponse[]>("api/capture/v1/file", content, "admin");

// No authentication
var response = await PostMultipartAsync<UploadResponse[]>("api/public/upload", content);
```

Responses are mapped through the same `ProcessResponse` pipeline as JSON `PostAsync` calls (status code, body, headers, validation errors).

### Aspire E2E Testing (Playwright)

For full-stack E2E tests against a .NET Aspire AppHost, use `RbkAspireTestingServer<TAppHost>` together with `RbkPlaywrightTestBase<TAppHost>`.

Project-specific settings are **constants for the entire test project**. Configure them once in a fixture subclass — the same pattern as `Demo1TestingServer : RbkTestingServer<Program>`.

The backend must declare `.WithHttpsEndpoint(..., name: "https")`. The fixture always uses that endpoint and derives the Playwright API redirect origin from it via `GetEndpoint`.

#### AspireTestingOptions

| Option | Default | Description |
|--------|---------|-------------|
| `BackendResourceName` | `"backend"` | Aspire resource name for the API |
| `FrontendResourceName` | `"frontend"` | Aspire resource name for the frontend (waited on before tests) |
| `FrontendPort` | `4207` | localhost port where ng serve / npm start listens |
| `FrontendBasePath` | `null` | Optional path suffix (e.g. `/gcab`) |
| `AccessTokenStorageKey` | `"access_token"` | localStorage key for the JWT |
| `LoginPath` | `"/api/authentication/login"` | Backend login endpoint |

Frontend URL is built as `http://localhost:{FrontendPort}` + optional `FrontendBasePath`.

#### Fixture subclass (required for non-default config)

```csharp
public class MyApp_AspireTestingServer : RbkAspireTestingServer<MyApp_AppHost>
{
    protected override AspireTestingOptions Options => new()
    {
        BackendResourceName = "backend",
        FrontendResourceName = "frontend",
        FrontendPort = 4207,
        FrontendBasePath = "/gcab",
        LoginPath = "/api/ca/login",
        AccessTokenStorageKey = "gcab_access_token",
    };
}
```

#### Test class

```csharp
public class UserManagement_E2E_Tests : RbkPlaywrightTestBase<MyApp_AppHost>
{
    [ClassDataSource<MyApp_AspireTestingServer>(Shared = SharedType.PerClass)]
    public required override MyApp_AspireTestingServer Fixture { get; set; }

    [Test]
    public async Task Admin_Can_View_Users()
    {
        await Authenticate("admin", "default");
        // Page and Context are ready; FrontendUrl comes from the fixture
    }
}
```

#### AppHost contract

Resource names in the AppHost must match `AspireTestingOptions`:

```csharp
var backend = builder.AddProject<Projects.MyApp_Api>("backend")
    .WithHttpsEndpoint(port: 44301, name: "https");
var frontend = builder.AddExecutable("frontend", "npm", frontPath, "start", "--", "--port", "4207")
    .WithReference(backend)
    .WithExternalHttpEndpoints();
```

The fixture resolves the backend URL via `CreateHttpClient` and the API redirect origin from the **configured** `https` endpoint port in the AppHost (e.g. `44301` from `.WithHttpsEndpoint(port: 44301, name: "https")`). During tests Aspire may assign a different runtime port; Playwright redirects browser calls from the configured origin to the runtime backend URL.

Set `FrontendPort` to match the port passed to ng serve / npm start.

#### Execution-only environment variables

These control test **execution**, not application config:

- `E2E_HEADLESS` — browser headless mode (default: `true`)
- `E2E_SLOW_MO` — Playwright slow-motion delay in ms
- `E2E_SCREENSHOT_ALWAYS` — capture screenshots on every test

Use `TestSettings` on the test base class to override `Headless` and diagnostic logging per test class.

#### Troubleshooting

**Backend endpoint not found**

The backend must declare `.WithHttpsEndpoint(..., name: "https")`. The fixture always uses the endpoint named `"https"`.

**Frontend registered with `AddExecutable` + `npm start -- --port 4207`**

Set `FrontendPort` to the same port passed on the CLI. The fixture waits for the Aspire `frontend` resource to become healthy, then opens `http://localhost:{FrontendPort}`.

**AppHost E2E conditionals (`isE2EMode`, `DROP_DATABASE_ON_START`, etc.)**

`RbkAspireTestingServer` starts the AppHost with `--environment=Testing` and sets `E2E_TESTING=true` before launch. Your AppHost can keep checking either flag:

```csharp
var isE2EMode = builder.Environment.EnvironmentName == "Testing"
    || Environment.GetEnvironmentVariable("E2E_TESTING") == "true";
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

### Outbound HTTP clients in integration tests

Integration tests often need to control outbound HTTP that the API under test makes — either to **external systems** (stub the response) or to a **sibling API** in the same solution (route through another `RbkTestingServer`). Both paths go through a testing `IHttpClientFactory` that resolves **named** clients.

#### Prerequisite — named HttpClients

Every outbound client in the application under test **must** be registered as a named `HttpClient`. The testing factory replaces `IHttpClientFactory` and looks up clients by that name:

```csharp
// Production / Program.cs — required pattern
builder.Services.AddHttpClient<INetworkDownloaderClient, NetworkDownloaderClient>(
    nameof(INetworkDownloaderClient));

builder.Services.AddHttpClient<IProcessingClient, ProcessingClient>(
    nameof(IProcessingClient));
```

The implementation **must** take `HttpClient` in its constructor (typed client pattern). Do not inject `IHttpClientFactory` directly into the client class when using `AddHttpClient<TClient, TImplementation>`:

```csharp
// Correct
public class ProcessingClient(HttpClient httpClient) : IProcessingClient { ... }

// Incorrect — breaks typed client registration
public class ProcessingClient(IHttpClientFactory factory) : IProcessingClient { ... }
```

Unnamed `AddHttpClient` / typed clients without a name will not resolve correctly once the testing factory is active. Use the same name in tests (`nameof(IClient)` by default for `AddMockHttpClient`).

#### Situation A — External HTTP (stub handler)

Use when the API calls an external service and you want production typed-client code to run, but control only the HTTP response.

**1. Register in `ConfigureTestServices`:**

```csharp
public class GlobalApiTestingServer : RbkTestingServer<Program>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        // Auto-wires CustomHttpClientFactory; name defaults to nameof(TClient)
        AddMockHttpClient<INetworkDownloaderClient, NetworkDownloaderClient>(services);
        AddMockHttpClient<IProteusAuthApiClient, ProteusAuthApiClient>(services);
    }
}
```

**2. Configure responses per test inside `HttpMockScope`:**

```csharp
[Test]
public async Task CaptureLink_DownloadsDocument()
{
    using var _ = GlobalApiTestingServer.HttpMockScope();

    var fileContent = new ByteArrayContent(File.ReadAllBytes("doc.pdf"));
    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");

    GlobalApiTestingServer.MockHttpGet<INetworkDownloaderClient>()
        .ReturnsSuccess(fileContent);

    // Or match with a predicate on the full request URL (omit the argument to match any URL):
    // GlobalApiTestingServer.MockHttpGet<IDocumentDownloader>(url => url.Contains("target-doc"))
    //     .ReturnsSuccess(fileContent);

    var response = await GlobalApiTestingServer.PostAsync<CaptureLink.Response>(
        "api/capture/v1/link", request, apiKey);

    response.ShouldBeSuccess();
}
```

Fluent helpers:

| Method | Purpose |
|--------|---------|
| `MockHttpGet<T>(urlMatcher?)` | Stub GET; omit `urlMatcher` to match any URL |
| `MockHttpPost<T>(urlMatcher?)` | Stub POST; omit `urlMatcher` to match any URL |
| `MockHttpCall<T>(method, urlMatcher?)` | Stub any verb; omit `urlMatcher` to match any URL |
| `.ReturnsSuccess(...)` | 200 with `HttpContent`, `byte[]`, or `string` |
| `.ReturnsBadRequest(...)` | 400 |
| `.ReturnsUnauthorized()` | 401 |
| `.Returns(statusCode, ...)` | Arbitrary status |

**Parallelism:** mock rules live in an `AsyncLocal` scope. `RbkTestingServer` sets `TestServer.PreserveExecutionContext = true` during initialization so the scope flows from your test into the in-process API pipeline. Keep arrange + act inside `HttpMockScope`. Use `[NotInParallel]` only if the app fires outbound HTTP on background threads / `Task.Run` that break execution context flow. Unmatched calls throw with the list of registered rules.

#### Situation B — Sibling API in the same solution

Use when API A calls API B and both run as test servers (e.g. Global → Processing).

**1. Register a named placeholder on the caller:**

```csharp
protected override void ConfigureTestServices(IServiceCollection services)
{
    AddMockHttpClient<INetworkDownloaderClient, NetworkDownloaderClient>(services);
    AddNamedHttpClient(services, nameof(IProcessingClient)); // placeholder until bound
}
```

**2. After both servers initialize, bind the live client (usually once in an ordered setup test):**

```csharp
[Test, NotInParallel(Order = 20)]
public async Task Prepare_Api_Clients()
{
    var client = ProcessingApiTestingServer.CreateClient();
    client.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, "valid-service-key");
    GlobalApiTestingServer.SetNamedHttpClient(nameof(IProcessingClient), client);
}
```

`SetNamedHttpClient` replaces the `HttpClient` in the shared factory dictionary for that fixture (process-wide for the shared server instance). There is no automatic wiring between two `RbkTestingServer` instances — binding stays explicit.

If you forget to bind a placeholder client, the first outbound call throws a clear error: *Named HttpClient "…" has not been bound. Call SetNamedHttpClient(…)*.

Downstream servers can still use Situation A for *their* external calls:

```csharp
using var _ = ProcessingApiTestingServer.HttpMockScope();
ProcessingApiTestingServer.MockHttpGet<IDocumentDownloader>()
    .ReturnsSuccess(fileContent);
```

#### Situation B — Bidirectional sibling calls (Demo1 ↔ Demo2)

When **both** APIs call each other, each caller needs its own named placeholder and binding. See `Demo.MultipleApis.Tests` for a full working example.

**Production — each API registers its outbound client and exposes an integration endpoint:**

```csharp
// Demo1/Program.cs
builder.Services.AddHttpClient<IDemo2ApiClient, Demo2ApiClient>(nameof(IDemo2ApiClient))
    .ConfigureHttpClient((sp, client) =>
    {
        client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["Demo2Api:BaseUrl"]!);
    });

// Demo2/Program.cs
builder.Services.AddHttpClient<IDemo1ApiClient, Demo1ApiClient>(nameof(IDemo1ApiClient))
    .ConfigureHttpClient((sp, client) =>
    {
        client.BaseAddress = new Uri(sp.GetRequiredService<IConfiguration>()["Demo1Api:BaseUrl"]!);
    });
```

**Test servers — register a placeholder on each caller:**

```csharp
// Demo1TestingServer
protected override void ConfigureTestServices(IServiceCollection services)
{
    AddNamedHttpClient(services, nameof(IDemo2ApiClient));
}

// Demo2TestingServer
protected override void ConfigureTestServices(IServiceCollection services)
{
    AddNamedHttpClient(services, nameof(IDemo1ApiClient));
}
```

**Setup test — bind both directions before any cross-API test runs:**

```csharp
[Test, NotInParallel(Order = 10)]
public async Task Bind_Cross_Api_Clients()
{
    var demo2Client = Demo2Server.CreateClient();
    Demo1Server.SetNamedHttpClient(nameof(IDemo2ApiClient), demo2Client);

    var demo1Client = Demo1Server.CreateClient();
    Demo2Server.SetNamedHttpClient(nameof(IDemo1ApiClient), demo1Client);
}

[Test, NotInParallel(Order = 20)]
public async Task Demo1_Calls_Demo2()
{
    var response = await Demo1Server.GetAsync<IntegrationResponse>("/integration/demo2/anonymous");
    response.ShouldBeSuccess(out var payload);
    payload.Message.ShouldBe("Anonymous");
}

[Test, NotInParallel(Order = 30)]
public async Task Demo2_Calls_Demo1()
{
    var response = await Demo2Server.GetAsync<IntegrationResponse>("/integration/demo1/anonymous");
    response.ShouldBeSuccess(out var payload);
    payload.Message.ShouldBe("Anonymous");
}
```

For protected sibling endpoints, add the target API's integration key to the bound client before `SetNamedHttpClient`:

```csharp
var serviceClient = ServiceServer.CreateClient();
serviceClient.DefaultRequestHeaders.Add(RbkAuthenticationSchemes.API_KEY, "valid-service-key");
GlobalApiTestingServer.SetNamedHttpClient(nameof(IProcessingClient), serviceClient);
```

#### Multi-API configuration (`appsettings.Testing.json` collision)

When a test project references **two or more Web SDK projects**, MSBuild copies all `appsettings*.json` files into the **same output folder**. Files with the same name collide — only the last one copied survives. Single-API tests are unaffected.

**Solution:** copy each API's settings into a unique subfolder and override the config path in each `RbkTestingServer` subclass.

**1. Test project `.csproj` — copy JSONs with unique destinations:**

```xml
<ItemGroup>
  <None Include="..\Demo1\appsettings.json"
        Link="Config\Demo1\appsettings.json"
        CopyToOutputDirectory="PreserveNewest" />
  <None Include="..\Demo1\appsettings.Testing.json"
        Link="Config\Demo1\appsettings.Testing.json"
        CopyToOutputDirectory="PreserveNewest" />
  <None Include="..\Demo2\appsettings.json"
        Link="Config\Demo2\appsettings.json"
        CopyToOutputDirectory="PreserveNewest" />
  <None Include="..\Demo2\appsettings.Testing.json"
        Link="Config\Demo2\appsettings.Testing.json"
        CopyToOutputDirectory="PreserveNewest" />
</ItemGroup>
```

The `Link=` attribute keeps the source files in the API projects — no duplication of config content.

**2. Override config hooks in each testing server:**

```csharp
public abstract class MultiApiTestingServerBase<TProgram> : RbkTestingServer<TProgram> where TProgram : class
{
    protected abstract string ConfigFolderName { get; }

    protected override string GetConfigurationBasePath()
        => Path.Combine(base.GetConfigurationBasePath(), "Config", ConfigFolderName);

    protected override IEnumerable<string> GetTestingConfigurationFiles()
        => ["appsettings.json", "appsettings.Testing.json"];
}

public class Demo1TestingServer : MultiApiTestingServerBase<Demo1.Program>
{
    protected override string ConfigFolderName => "Demo1";
    // ...
}
```

**3. Configuration precedence** (lowest → highest):

| Order | Source |
|-------|--------|
| 1 | Files from `GetTestingConfigurationFiles()` (in order) |
| 2 | `ConfigureAppConfiguration` hook (optional extra JSON/env vars) |
| 3 | `ConfigureInMemoryOverrides()` |

**Notes:**

- `ExcludeAssets=contentfiles` on `ProjectReference` is **not required**. Collided files at the output root are simply ignored once each server loads from its own subfolder.
- Single-API test projects can keep the default behavior (`appsettings.Testing.json` at the output root) without any override.
- Use `ConfigureInMemoryOverrides()` only for small, dynamic adjustments — not to compensate for missing JSON files.

Reference implementation: `Demo.MultipleApis.Tests` in the rbkApiModules repository.

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

`RbkTestingServer<TProgram>` loads configuration from the **Testing** environment. By default it reads `appsettings.Testing.json` from the hosted API assembly folder (typically the test project output).

Override these hooks when hosting multiple APIs from the same test project:

```csharp
protected override string GetConfigurationBasePath()
    => Path.Combine(base.GetConfigurationBasePath(), "Config", "MyApi");

protected override IEnumerable<string> GetTestingConfigurationFiles()
    => ["appsettings.json", "appsettings.Testing.json"];

protected override void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder config)
{
    // Optional: add extra sources between JSON files and in-memory overrides
}

protected override IEnumerable<KeyValuePair<string, string>> ConfigureInMemoryOverrides()
    => [];
```

See [Multi-API configuration](#multi-api-configuration-appsettingstestingjson-collision) for the full appsettings collision workaround.

For service overrides:

```csharp
public class UserControllerTests : RbkTestingServer<Program>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services.AddScoped<IEmailService, MockEmailService>();
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

See [Outbound HTTP clients in integration tests](#outbound-http-clients-in-integration-tests) for `AddMockHttpClient`, `AddNamedHttpClient`, `SetNamedHttpClient`, and the fluent `MockHttpGet` / `MockHttpPost` API.

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

1. **Named clients**: Register production HttpClients with `nameof(IClient)` so the testing factory can resolve them
2. **Fluent stubs**: Prefer `HttpMockScope` + `MockHttpGet` / `MockHttpPost` over hand-rolled Moq handler setups
3. **Scope arrange + act**: Keep outbound mock rules and the API call inside the same `HttpMockScope`
4. **Sibling APIs**: Use `AddNamedHttpClient` + `SetNamedHttpClient(otherServer.CreateClient())` once during ordered setup
5. **Parallelism**: Rely on `AsyncLocal` isolation; fall back to `[NotInParallel]` for background outbound HTTP

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
- Shouldly
- TUnit
- MimeKit
- Aspire.Hosting.Testing
- Microsoft.Playwright

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