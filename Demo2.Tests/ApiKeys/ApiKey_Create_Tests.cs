using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_Create_Tests
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
    public async Task Create_Returns_400_When_ClaimIds_Is_Empty()
    {
        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid>()
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "At least one claim is required.");
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Create_Returns_400_When_ClaimIds_Has_Duplicates()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid> { claimId, claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Duplicate claim identifiers in the request.");
    }

    [Test, NotInParallel(Order = 4)]
    public async Task Create_Returns_400_When_Claim_Does_Not_Exist()
    {
        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid> { Guid.NewGuid() }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "One or more claims were not found.");
    }

    [Test, NotInParallel(Order = 5)]
    public async Task Create_Returns_400_When_Claim_Is_Not_Allowed_On_ApiKeys()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS).Id;

        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim 'MANAGE_USERS' is not allowed on API keys.");
    }

    [Test, NotInParallel(Order = 6)]
    public async Task Create_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 7)]
    public async Task Create_Returns_Raw_Key_And_Correct_Metadata()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "My Test Key",
            ExpirationDate = null,
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.Id.ShouldNotBe(Guid.Empty);
        result.RawKey.ShouldNotBeNullOrWhiteSpace();
        result.RawKey.ShouldStartWith("rbk_live_");
        result.KeyPrefix.ShouldBe("rbk_live_");
        result.Name.ShouldBe("My Test Key");
        result.ExpirationDate.ShouldBeNull();
        result.TenantId.ShouldBeNull();
        result.AssignedClaims.Count.ShouldBe(1);
        result.AssignedClaims[0].Identification.ShouldBe("DEMO2_INTEGRATION");
        result.AssignedClaims[0].AllowApiKeyUsage.ShouldBeTrue();

        var dbKey = TestingServer.CreateContext()
            .Set<EntityApiKey>().Find(result.Id);

        dbKey.ShouldNotBeNull();
        dbKey!.Name.ShouldBe("My Test Key");
        dbKey.IsActive.ShouldBeTrue();
        dbKey.TenantId.ShouldBeNull();
        dbKey.ExpirationDate.ShouldBeNull();
        dbKey.LastUsedAt.ShouldBeNull();
        dbKey.RevokeDate.ShouldBeNull();
        dbKey.RevokeReason.ShouldBeNull();
    }

    [Test, NotInParallel(Order = 8)]
    public async Task List_Includes_New_Key_After_Creation()
    {
        var response = await TestingServer.GetAsync<List<ApiKeyDetails>>(
            "api/authorization/api-keys", "superuser");

        response.ShouldBeSuccess(out var keys);

        keys.Count.ShouldBe(2);
        keys.ShouldContain(k => k.Name == "Demo2 integration");
        keys.ShouldContain(k => k.Name == "My Test Key");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
