using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[NotInParallel(nameof(ApiKey_Behavior_Tests))]
public class ApiKey_Behavior_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    // ────────────────────────── LastUsedAt ──────────────────────────

    [Test, NotInParallel(Order = 2)]
    public async Task LastUsedAt_Is_Null_Before_First_Authentication()
    {
        var dbKey = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        dbKey.LastUsedAt.ShouldBeNull();
    }

    [Test, NotInParallel(Order = 3)]
    public async Task LastUsedAt_Is_Set_After_First_Successful_Authentication()
    {
        var timeBefore = DateTime.UtcNow;

        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        var dbKey = TestingServer.CreateContext()
            .Set<EntityApiKey>().Find(
                TestingServer.CreateContext().Set<EntityApiKey>()
                    .First(x => x.Name == "Demo2 integration").Id);

        dbKey!.LastUsedAt.ShouldNotBeNull();
        dbKey.LastUsedAt!.Value.ShouldBeGreaterThanOrEqualTo(timeBefore);
    }

    [Test, NotInParallel(Order = 4)]
    public async Task LastUsedAt_Does_Not_Change_On_Immediate_Subsequent_Authentication()
    {
        var keyId = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration").Id;

        // Ensure there's a LastUsedAt value from the previous test
        var firstLastUsedAt = TestingServer.CreateContext()
            .Set<EntityApiKey>().Find(keyId)!.LastUsedAt;

        firstLastUsedAt.ShouldNotBeNull();

        // Authenticate again immediately — the throttle cache entry is still alive
        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        var secondLastUsedAt = TestingServer.CreateContext()
            .Set<EntityApiKey>().Find(keyId)!.LastUsedAt;

        // The throttler prevents writing LastUsedAt within the min-interval window
        secondLastUsedAt.ShouldBe(firstLastUsedAt);
    }

    // ────────────────────────── Usage statistics ──────────────────────────

    [Test, NotInParallel(Order = 5)]
    public async Task Successful_Authentication_Creates_A_Usage_Entry_For_Today()
    {
        var keyId = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration").Id;

        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var usageRow = TestingServer.CreateContext()
            .Set<ApiKeyUsageByDay>()
            .FirstOrDefault(x => x.ApiKeyId == keyId && x.Date == today);

        usageRow.ShouldNotBeNull();
        usageRow!.Count.ShouldBeGreaterThanOrEqualTo(1);
    }

    [Test, NotInParallel(Order = 6)]
    public async Task Multiple_Authentications_On_Same_Day_Aggregate_Into_One_Row()
    {
        var keyId = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration").Id;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var countBefore = TestingServer.CreateContext()
            .Set<ApiKeyUsageByDay>()
            .Where(x => x.ApiKeyId == keyId && x.Date == today)
            .Select(x => x.Count)
            .FirstOrDefault();

        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));
        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));
        await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        var rows = TestingServer.CreateContext()
            .Set<ApiKeyUsageByDay>()
            .Where(x => x.ApiKeyId == keyId && x.Date == today)
            .ToList();

        rows.Count.ShouldBe(1);
        rows[0].Count.ShouldBe(countBefore + 3);
    }

    // ────────────────────────── Cache invalidation ──────────────────────────

    [Test, NotInParallel(Order = 7)]
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
            Name = "Key For Deactivation Test",
            IsActive = false,
            ClaimIds = new List<Guid> { claimId }
        };

        var updateResponse = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{created.Id}", updateRequest, "superuser");

        updateResponse.ShouldBeSuccess(out var updated);
        updated.IsActive.ShouldBeFalse();

        // The next request should fail because the cache was cleared and DB shows inactive
        var rejectedResponse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        rejectedResponse.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 8)]
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
        var revokeRequest = new RevokeApiKey.Request { Reason = "Compromised during test" };

        var revokeResponse = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{created.Id}/revoke", revokeRequest, "superuser");

        revokeResponse.ShouldBeSuccess(out var revoked);
        revoked.IsActive.ShouldBeFalse();
        revoked.RevokeReason.ShouldBe("Compromised during test");

        // The next request should be rejected immediately (cache cleared)
        var rejectedResponse = await TestingServer.GetAsync("demo/apikey", new ApiKey(rawKey));
        rejectedResponse.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    // ────────────────────────── Expiration ──────────────────────────

    [Test, NotInParallel(Order = 9)]
    public async Task Expired_Key_Is_Rejected()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        // Create a key already past its expiration date
        var createRequest = new CreateApiKey.Request
        {
            Name = "Expired Key",
            ExpirationDate = DateTime.UtcNow.AddSeconds(-1),
            ClaimIds = new List<Guid> { claimId }
        };

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        createResponse.ShouldBeSuccess(out var created);

        // The key is past expiration; the validator must reject it
        var response = await TestingServer.GetAsync("demo/apikey", new ApiKey(created.RawKey));

        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }

    [Test, NotInParallel(Order = 10)]
    public async Task Key_With_Future_Expiration_Is_Accepted()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var createRequest = new CreateApiKey.Request
        {
            Name = "Future Expiry Key",
            ExpirationDate = DateTime.UtcNow.AddDays(365),
            ClaimIds = new List<Guid> { claimId }
        };

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Handler.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        createResponse.ShouldBeSuccess(out var created);

        var response = await TestingServer.GetAsync("demo/apikey", new ApiKey(created.RawKey));

        response.Code.ShouldBe(HttpStatusCode.OK);
    }
}
