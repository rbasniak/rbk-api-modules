using System.Net;
using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_Update_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
        await TestingServer.CacheCredentialsAsync("admin1", "buzios");
        await TestingServer.CacheCredentialsAsync("john.doe", "buzios");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var createRequest = new CreateApiKey.Request
        {
            Name = "My Test Key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        response.ShouldBeSuccess();
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Update_Returns_400_When_ClaimIds_Is_Empty()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid>()
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "At least one claim is required.");
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Update_Returns_400_When_Key_Does_Not_Exist()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id= Guid.NewGuid(),
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "API key not found.");
    }

    [Test, NotInParallel(Order = 4)]
    public async Task Update_Returns_400_When_Claim_Is_Not_Allowed_On_ApiKeys()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var restrictedClaimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS).Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { restrictedClaimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Claim 'MANAGE_USERS' is not allowed on API keys.");
    }

    [Test, NotInParallel(Order = 5)]
    public async Task Update_Returns_403_For_User_Without_CAN_MANAGE_APIKEYS()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "My Test Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "john.doe");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 6)]
    public async Task Update_Returns_403_When_Tenant_Admin_Targets_Global_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 integration");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Demo2 integration",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 7)]
    public async Task Update_As_Tenant_Admin_Succeeds_For_Own_Tenant_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 buzios manage");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Demo2 buzios manage renamed",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeSuccess(out var result);
        result.Name.ShouldBe("Demo2 buzios manage renamed");
    }

    [Test, NotInParallel(Order = 8)]
    public async Task Update_Returns_403_When_Tenant_Admin_Targets_Other_Tenant_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Demo2 un-bs manage");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Demo2 un-bs manage",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 9)]
    public async Task Update_Can_Rename_A_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "My Test Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.Name.ShouldBe("Renamed Key");
        result.IsActive.ShouldBeTrue();

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.Name.ShouldBe("Renamed Key");
    }

    [Test, NotInParallel(Order = 10)]
    public async Task Update_Can_Set_Expiration_Date()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var expiration = DateTime.UtcNow.AddDays(30);

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ExpirationDate = expiration,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.ExpirationDate.ShouldNotBeNull();
        result.ExpirationDate!.Value.ShouldBe(expiration, TimeSpan.FromSeconds(5));

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.ExpirationDate.ShouldNotBeNull();
    }

    [Test, NotInParallel(Order = 11)]
    public async Task Update_Can_Deactivate_A_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = false,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.IsActive.ShouldBeFalse();

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.IsActive.ShouldBeFalse();
    }

    [Test, NotInParallel(Order = 12)]
    public async Task Update_Can_Reactivate_A_Deactivated_Key()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            $"api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.IsActive.ShouldBeTrue();
    }

    [Test, NotInParallel(Order = 13)]
    public async Task Update_With_Explicit_Rate_Limits_Returns_And_Persists_Them()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 55,
            BurstLimit = 200
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.RequestsPerMinute.ShouldBe(55);
        result.BurstLimit.ShouldBe(200);

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(key.Id);
        dbKey!.RequestsPerMinute.ShouldBe(55);
        dbKey.BurstLimit.ShouldBe(200);
    }

    [Test, NotInParallel(Order = 14)]
    public async Task Update_Returns_400_When_Burst_Limit_Less_Than_Requests_Per_Minute()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 80,
            BurstLimit = 40
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Burst limit must be greater than or equal to requests per minute.");
    }

    [Test, NotInParallel(Order = 15)]
    public async Task Update_With_Only_Requests_Per_Minute_Keeps_Burst_When_Still_Valid()
    {
        var key = TestingServer.CreateContext()
            .Set<EntityApiKey>().First(x => x.Name == "Renamed Key");

        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new UpdateApiKey.Request
        {
            Id = key.Id,
            Name = "Renamed Key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 60,
            BurstLimit = null
        };

        var response = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.RequestsPerMinute.ShouldBe(60);
        result.BurstLimit.ShouldBe(200);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
