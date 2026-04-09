using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_List_Tests
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
    public async Task List_Returns_The_Seeded_Integration_Key()
    {
        var response = await TestingServer.GetAsync<List<ApiKeyDetails>>("api/authorization/api-keys", "superuser");

        response.ShouldBeSuccess(out var keys);

        keys.Count.ShouldBe(1);
        keys[0].Name.ShouldBe("Demo2 integration");
        keys[0].KeyPrefix.ShouldBe("rbk_live_");
        keys[0].IsActive.ShouldBeTrue();
        keys[0].ExpirationDate.ShouldBeNull();
        keys[0].TenantId.ShouldBeNull();
        keys[0].RevokeDate.ShouldBeNull();
        keys[0].RevokeReason.ShouldBeNull();
        keys[0].AssignedClaims.Count.ShouldBe(1);
        keys[0].AssignedClaims[0].Identification.ShouldBe("DEMO2_INTEGRATION");
        keys[0].AssignedClaims[0].AllowApiKeyUsage.ShouldBeTrue();
    }

    [Test, NotInParallel(Order = 3)]
    public async Task List_Returns_401_Without_Authentication()
    {
        var response = await TestingServer.GetAsync("api/authorization/api-keys");

        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 4)]
    public async Task List_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var response = await TestingServer.GetAsync("api/authorization/api-keys", "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
