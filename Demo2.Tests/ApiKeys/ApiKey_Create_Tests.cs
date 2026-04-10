using System.Net;
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
        await TestingServer.CacheCredentialsAsync("john.doe", "buzios");
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Create_Returns_400_When_ClaimIds_Is_Empty()
    {
        var request = new CreateApiKey.Request
        {
            Name = "Test Key",
            ClaimIds = new List<Guid>()
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
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

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
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

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
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

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
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

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "john.doe");

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

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
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
        result.RequestsPerMinute.ShouldBe(600);
        result.BurstLimit.ShouldBe(600);

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
        dbKey.RequestsPerMinute.ShouldBe(600);
        dbKey.BurstLimit.ShouldBe(600);
    }

    [Test, NotInParallel(Order = 8)]
    public async Task List_Includes_New_Key_After_Creation()
    {
        var response = await TestingServer.GetAsync<List<ApiKeyDetails>>(
            "api/authorization/api-keys", "superuser");

        response.ShouldBeSuccess(out var keys);

        keys.Count.ShouldBe(6);
        keys.ShouldContain(k => k.Name == "Demo2 integration");
        keys.ShouldContain(k => k.Name == "My Test Key");
    }

    [Test, NotInParallel(Order = 9)]
    public async Task Create_As_Tenant_Admin_With_TenantId_Succeeds_For_Own_Tenant()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Admin1 tenant key",
            TenantId = "BUZIOS",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeSuccess(out var result);
        result.TenantId.ShouldBe("BUZIOS");
    }

    [Test, NotInParallel(Order = 10)]
    public async Task Create_As_Tenant_Admin_Global_Key_Returns_403_Without_Cross_Tenant_Claim()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Should fail global",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 11)]
    public async Task Create_As_Tenant_Admin_Wrong_Tenant_Returns_403()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Wrong tenant",
            TenantId = "UN-BS",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "admin1");

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 12)]
    public async Task Create_With_Global_Manage_Only_Api_Key_And_Null_Tenant_Returns_403()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "From manage-only key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, new ApiKey(Demo2TestApiKeys.GlobalManageOnly));

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 13)]
    public async Task Create_With_Global_Manage_And_Cross_Api_Key_And_Null_Tenant_Succeeds()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "From full global key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, new ApiKey(Demo2TestApiKeys.GlobalManageAndCrossTenant));

        response.ShouldBeSuccess(out var result);
        result.TenantId.ShouldBeNull();
    }

    [Test, NotInParallel(Order = 14)]
    public async Task Create_With_Buzios_Api_Key_For_Buzios_Succeeds()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "From buzios api key",
            TenantId = "BUZIOS",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, new ApiKey(Demo2TestApiKeys.BuziosManage));

        response.ShouldBeSuccess(out var result);
        result.TenantId.ShouldBe("BUZIOS");
    }

    [Test, NotInParallel(Order = 15)]
    public async Task Create_With_Buzios_Api_Key_Global_Tenant_Returns_403()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Buzios key global fail",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, new ApiKey(Demo2TestApiKeys.BuziosManage));

        response.ShouldBeForbidden();
    }

    [Test, NotInParallel(Order = 16)]
    public async Task Create_As_Superuser_Can_Create_Tenant_Scoped_Key_For_Another_Tenant()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Superuser created UN-BS key",
            TenantId = "UN-BS",
            ClaimIds = new List<Guid> { claimId }
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);
        result.TenantId.ShouldBe("UN-BS");
    }

    [Test, NotInParallel(Order = 17)]
    public async Task Create_With_Explicit_Rate_Limits_Returns_And_Persists_Them()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Explicit rate limits key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 42,
            BurstLimit = 120
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldBeSuccess(out var result);

        result.RequestsPerMinute.ShouldBe(42);
        result.BurstLimit.ShouldBe(120);

        var dbKey = TestingServer.CreateContext().Set<EntityApiKey>().Find(result.Id);
        dbKey.ShouldNotBeNull();
        dbKey!.RequestsPerMinute.ShouldBe(42);
        dbKey.BurstLimit.ShouldBe(120);
    }

    [Test, NotInParallel(Order = 18)]
    public async Task Create_Returns_400_When_Burst_Limit_Less_Than_Requests_Per_Minute()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Invalid burst key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 100,
            BurstLimit = 50
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, "Burst limit must be greater than or equal to requests per minute.");
    }

    [Test, NotInParallel(Order = 19)]
    public async Task Create_Returns_400_When_Requests_Per_Minute_Out_Of_Range()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var request = new CreateApiKey.Request
        {
            Name = "Invalid rpm key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 0,
            BurstLimit = 1
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", request, "superuser");

        response.ShouldHaveErrors(HttpStatusCode.BadRequest, $"Requests per minute must be between {EntityApiKey.MinRequestsPerMinute} and {EntityApiKey.MaxRequestsPerMinute}.");
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
