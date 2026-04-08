using System.Net;
using System.Text.Json.Serialization;
using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;

namespace rbkApiModules.Identity.Tests.MixedAuthentication;

[NotInParallel(nameof(Demo_Authentication_Endpoints_Tests))]
public class Demo_Authentication_Endpoints_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    /// <summary>
    /// Cache superuser (JWT endpoint) and admin1 (role endpoint - has MANAGE_USERS) using Windows auth (no password).
    /// </summary>
    [Test]
    public async Task Seed_Credentials()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "buzios");
    }

    [Test]
    public async Task ApiKey_Endpoint_Returns_401_When_No_Header()
    {
        var response = await TestingServer.GetAsync("demo/apikey");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task ApiKey_Endpoint_Returns_401_When_Invalid_Key()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/apikey", new ApiKey("wrong-key"));
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task ApiKey_Endpoint_Returns_200_When_Valid_Key()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("API key accepted");
    }

    [Test]
    public async Task Jwt_Endpoint_Returns_401_When_No_Auth()
    {
        var response = await TestingServer.GetAsync("demo/jwt");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Jwt_Endpoint_Returns_200_When_Authenticated_With_Superuser()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/jwt", "superuser");
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("JWT accepted");
    }

    [Test]
    public async Task Role_Endpoint_Returns_401_When_No_Auth()
    {
        var response = await TestingServer.GetAsync("demo/role");
        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async Task Role_Endpoint_Returns_200_When_User_Has_ManageUsers_Claim()
    {
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/role", "admin1");
        response.ShouldBeSuccess<DemoMessageResponse>(out var data);
        data.Message.ShouldBe("Role check passed");
    }

    [Test]
    public async Task Role_Endpoint_Returns_403_When_User_Without_ManageUsers_Claim()
    {
        await TestingServer.CacheCredentialsAsync("john.doe", "buzios");
        var response = await TestingServer.GetAsync<DemoMessageResponse>("demo/role", "john.doe");
        response.Code.ShouldBe(HttpStatusCode.Forbidden);
    }

    [Test]
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
