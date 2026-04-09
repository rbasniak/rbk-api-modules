using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_Revoke_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "buzios");
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Revoke_Returns_400_Without_Reason()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = string.Empty, Id = key.Id };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Revoke reason is required.");
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Revoke_Returns_400_When_Key_Does_Not_Exist()
    {
        var request = new RevokeApiKey.Request { Reason = "Not needed", Id = Guid.NewGuid() };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key not found.");
    }

    [Test, NotInParallel(Order = 4)]
    public async Task Revoke_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = "Should be forbidden", Id = key.Id };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 5)]
    public async Task Revoke_Sets_Key_Inactive_With_Reason_And_Date()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = "Key compromised during testing", Id = key.Id };

        var timeBefore = DateTime.UtcNow;

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.IsActive.ShouldBeFalse();
        result.RevokeDate.ShouldNotBeNull();
        result.RevokeDate!.Value.ShouldBeGreaterThanOrEqualTo(timeBefore);
        result.RevokeReason.ShouldBe("Key compromised during testing");

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.IsActive.ShouldBeFalse();
        dbKey.RevokeDate.ShouldNotBeNull();
        dbKey.RevokeReason.ShouldBe("Key compromised during testing");
    }

    [Test, NotInParallel(Order = 6)]
    public async Task Revoke_Returns_400_When_Already_Revoked()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        key.IsActive.ShouldBeFalse();

        var request = new RevokeApiKey.Request { Reason = "Attempting second revoke", Id = key.Id };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key is already revoked.");
    }

    [Test, NotInParallel(Order = 7)]
    public async Task Revoked_Key_Is_Rejected_For_Authentication()
    {
        var response = await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
