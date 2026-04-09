using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[NotInParallel(nameof(ApiKey_Management_Tests))]
public class ApiKey_Management_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "buzios");
    }

    // ────────────────────────── List ──────────────────────────

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

    // ────────────────────────── Create – validation ──────────────────────────

    [Test, NotInParallel(Order = 5)]
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

    [Test, NotInParallel(Order = 6)]
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

    [Test, NotInParallel(Order = 7)]
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

    [Test, NotInParallel(Order = 8)]
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

    [Test, NotInParallel(Order = 9)]
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

    // ────────────────────────── Create – success ──────────────────────────

    [Test, NotInParallel(Order = 10)]
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

    [Test, NotInParallel(Order = 11)]
    public async Task List_Includes_New_Key_After_Creation()
    {
        var response = await TestingServer.GetAsync<List<ApiKeyDetails>>(
            "api/authorization/api-keys", "superuser");

        response.ShouldBeSuccess(out var keys);

        keys.Count.ShouldBe(2);
        keys.ShouldContain(k => k.Name == "Demo2 integration");
        keys.ShouldContain(k => k.Name == "My Test Key");
    }

    // ────────────────────────── Update – validation ──────────────────────────

    [Test, NotInParallel(Order = 12)]
    public async Task Update_Returns_400_When_ClaimIds_Is_Empty()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var request = new UpdateApiKey.Request
        {
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid>()
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "At least one claim is required.");
    }

    [Test, NotInParallel(Order = 13)]
    public async Task Update_Returns_400_When_Key_Does_Not_Exist()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{Guid.NewGuid()}", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key not found.");
    }

    [Test, NotInParallel(Order = 14)]
    public async Task Update_Returns_400_When_Claim_Is_Not_Allowed_On_ApiKeys()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var restrictedClaimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS).Id;

        var request = new UpdateApiKey.Request
        {
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { restrictedClaimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim 'MANAGE_USERS' is not allowed on API keys.");
    }

    [Test, NotInParallel(Order = 15)]
    public async Task Update_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "admin1");

        response.ShouldBeForbidden();
    }

    // ────────────────────────── Update – success ──────────────────────────

    [Test, NotInParallel(Order = 16)]
    public async Task Update_Can_Rename_A_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.Name.ShouldBe("Renamed Key");
        result.IsActive.ShouldBeTrue();

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.Name.ShouldBe("Renamed Key");
    }

    [Test, NotInParallel(Order = 17)]
    public async Task Update_Can_Set_Expiration_Date()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var expiration = DateTime.UtcNow.AddDays(30);

        var request = new UpdateApiKey.Request
        {
            Name = "Renamed Key",
            IsActive = true,
            ExpirationDate = expiration,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.ExpirationDate.ShouldNotBeNull();
        result.ExpirationDate!.Value.ShouldBe(expiration, TimeSpan.FromSeconds(5));

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.ExpirationDate.ShouldNotBeNull();
    }

    [Test, NotInParallel(Order = 18)]
    public async Task Update_Can_Deactivate_A_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Name = "Renamed Key",
            IsActive = false,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.IsActive.ShouldBeFalse();

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.IsActive.ShouldBeFalse();
    }

    [Test, NotInParallel(Order = 19)]
    public async Task Update_Can_Reactivate_A_Deactivated_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.IsActive.ShouldBeTrue();
    }

    // ────────────────────────── Revoke – validation ──────────────────────────

    [Test, NotInParallel(Order = 20)]
    public async Task Revoke_Returns_400_Without_Reason()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = string.Empty };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Revoke reason is required.");
    }

    [Test, NotInParallel(Order = 21)]
    public async Task Revoke_Returns_400_When_Key_Does_Not_Exist()
    {
        var request = new RevokeApiKey.Request { Reason = "Not needed" };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{Guid.NewGuid()}/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key not found.");
    }

    [Test, NotInParallel(Order = 22)]
    public async Task Revoke_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = "Should be forbidden" };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}/revoke", request, "admin1");

        response.ShouldBeForbidden();
    }

    // ────────────────────────── Revoke – success ──────────────────────────

    [Test, NotInParallel(Order = 23)]
    public async Task Revoke_Sets_Key_Inactive_With_Reason_And_Date()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var request = new RevokeApiKey.Request { Reason = "Key compromised during testing" };

        var timeBefore = DateTime.UtcNow;

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}/revoke", request, "superuser");

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

    [Test, NotInParallel(Order = 24)]
    public async Task Revoke_Returns_400_When_Already_Revoked()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        key.IsActive.ShouldBeFalse();

        var request = new RevokeApiKey.Request { Reason = "Attempting second revoke" };

        var response = await TestingServer.PostAsync<ApiKeyDetails>(
            $"api/authorization/api-keys/{key.Id}/revoke", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key is already revoked.");
    }

    [Test, NotInParallel(Order = 25)]
    public async Task Revoked_Key_Is_Rejected_For_Authentication()
    {
        var response = await TestingServer.GetAsync("demo/apikey", new ApiKey(DemoIntegrationApiKey.Value));

        response.Code.ShouldBe(HttpStatusCode.Unauthorized);
    }
}
