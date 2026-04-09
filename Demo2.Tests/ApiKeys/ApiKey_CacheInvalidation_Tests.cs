using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_CacheInvalidation_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Deactivated_Key_Is_Rejected_After_Cache_Invalidation()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        // Create a dedicated key so we don't affect the seeded one
        var createRequest = new CreateApiKey.Request
        {
            Name = "Key For Deactivation Test",
            ClaimIds = new List<Guid> { claimId }
        };

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        createResponse.ShouldBeSuccess(out var created);
        var rawKey = created.RawKey;

        // Prime the auth cache by using the key once
        var firstUse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        firstUse.Code.ShouldBe(HttpStatusCode.OK);

        // Deactivate the key — the update handler invalidates the auth cache
        var updateRequest = new UpdateApiKey.Request
        {
            Id = created.Id,
            Name = "Key For Deactivation Test",
            IsActive = false,
            ClaimIds = new List<Guid> { claimId }
        };

        var updateResponse = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", updateRequest, "superuser");

        updateResponse.ShouldBeSuccess(out var updated);
        updated.IsActive.ShouldBeFalse();

        // The next request should fail because the cache was cleared and DB shows inactive
        var rejectedResponse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        rejectedResponse.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Revoked_Key_Is_Rejected_After_Cache_Invalidation()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        // Create a dedicated key
        var createRequest = new CreateApiKey.Request
        {
            Name = "Key For Revoke Test",
            ClaimIds = new List<Guid> { claimId }
        };

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        createResponse.ShouldBeSuccess(out var created);
        var rawKey = created.RawKey;

        // Prime the auth cache
        var firstUse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        firstUse.Code.ShouldBe(HttpStatusCode.OK);

        // Revoke the key — the revoke handler invalidates the auth cache
        var revokeRequest = new RevokeApiKey.Request { Reason = "Compromised during test", Id = created.Id };

        var revokeResponse = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/revoke", revokeRequest, "superuser");

        revokeResponse.ShouldBeSuccess(out var revoked);
        revoked.IsActive.ShouldBeFalse();
        revoked.RevokeReason.ShouldBe("Compromised during test");

        // The next request should be rejected immediately (cache cleared)
        var rejectedResponse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        rejectedResponse.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
