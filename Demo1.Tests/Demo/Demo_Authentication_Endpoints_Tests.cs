using System.Net;
using System.Text.Json.Serialization;
using Demo1;
using Demo1.Tests;
using rbkApiModules.Commons.Testing;

namespace rbkApiModules.Identity.Tests.ApiKeyAuthentication;

[HumanFriendlyDisplayName]
public class Demo_Authentication_Endpoints_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// Cache superuser (JWT endpoint) and admin1 (role endpoint - has MANAGE_USERS) credentials.
    /// </summary>
    [Test, NotInParallel(Order = 1)]
    public async Task Seed_Credentials()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "123", "buzios");
    }

    [Test, NotInParallel(Order = 2)]
    public async Task ApiKey_Endpoint_Returns_401_When_No_Header()
    {
        var response = await TestingServer.GetAsync("demo/apikey");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 3)]
    public async Task ApiKey_Endpoint_Returns_401_When_Invalid_Key()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/apikey", new ApiKey("wrong-key"));
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 4)]
    public async Task ApiKey_Endpoint_Returns_200_When_Valid_Key()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("API key accepted");
    }

    [Test, NotInParallel(Order = 5)]
    public async Task Jwt_Endpoint_Returns_401_When_No_Auth()
    {
        var response = await TestingServer.GetAsync("demo/jwt");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 6)]
    public async Task Jwt_Endpoint_Returns_200_When_Authenticated_With_Superuser()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/jwt", "superuser");
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("JWT accepted");
    }

    [Test, NotInParallel(Order = 7)]
    public async Task Role_Endpoint_Returns_401_When_No_Auth()
    {
        var response = await TestingServer.GetAsync("demo/role");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 8)]
    public async Task Role_Endpoint_Returns_200_When_User_Has_ManageUsers_Claim()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/role", "admin1");
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("Role check passed");
    }

    [Test, NotInParallel(Order = 9)]
    public async Task Role_Endpoint_Returns_403_When_User_Without_ManageUsers_Claim()
    {
        await TestingServer.CacheCredentialsAsync("john.doe", "123", "buzios");
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/role", "john.doe");
        response.Code.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test, NotInParallel(Order = 10)]
    public async Task Anonymous_Endpoint_Returns_200_Without_Auth()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/anonymous");
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("Anonymous");
    }

    private class DemoMessageResponse
    {
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
