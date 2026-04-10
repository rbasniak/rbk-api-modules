using System.Net;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_RateLimit_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Create_Persists_Rate_Limits_And_Returns_Them()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var createRequest = new CreateApiKey.Request
        {
            Name = "Rate limit test key",
            TenantId = null,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 2,
            BurstLimit = 2
        };

        var response = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys", createRequest, "superuser");

        response.ShouldBeSuccess(out var created);
        created.RequestsPerMinute.ShouldBe(2);
        created.BurstLimit.ShouldBe(2);

        var entity = TestingServer.CreateContext().Set<EntityApiKey>().First(x => x.Id == created.Id);
        entity.RequestsPerMinute.ShouldBe(2);
        entity.BurstLimit.ShouldBe(2);
    }

    [Test, NotInParallel(Order = 3)]
    public async Task Third_Quick_Request_Returns_429_When_Burst_Is_Two()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys",
            new CreateApiKey.Request
            {
                Name = "Rate limit burst key",
                TenantId = null,
                ClaimIds = new List<Guid> { claimId },
                RequestsPerMinute = 2,
                BurstLimit = 2
            },
            "superuser");

        createResponse.ShouldBeSuccess(out var keyResult);
        var apiKeyHeader = new ApiKey(keyResult.RawKey);

        var first = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        var second = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        var third = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);

        first.Code.ShouldBe(HttpStatusCode.OK);
        second.Code.ShouldBe(HttpStatusCode.OK);
        third.Code.ShouldBe(HttpStatusCode.TooManyRequests);
    }

    [Test, NotInParallel(Order = 4)]
    public async Task Update_Higher_Limits_Allows_Burst_Again()
    {
        var claimId = TestingServer.CreateContext()
            .Set<Claim>().First(x => x.Identification == "DEMO2_INTEGRATION").Id;

        var createResponse = await TestingServer.PostAsync<CreateApiKey.Result>(
            "api/authorization/api-keys",
            new CreateApiKey.Request
            {
                Name = "Rate limit update key",
                TenantId = null,
                ClaimIds = new List<Guid> { claimId },
                RequestsPerMinute = 2,
                BurstLimit = 2
            },
            "superuser");

        createResponse.ShouldBeSuccess(out var created);
        var apiKeyHeader = new ApiKey(created.RawKey);

        await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        var blocked = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        blocked.Code.ShouldBe(HttpStatusCode.TooManyRequests);

        var updateRequest = new UpdateApiKey.Request
        {
            Id = created.Id,
            Name = "Rate limit update key",
            IsActive = true,
            ClaimIds = new List<Guid> { claimId },
            RequestsPerMinute = 100,
            BurstLimit = 100
        };

        var updateResponse = await TestingServer.PutAsync<ApiKeyDetails>(
            "api/authorization/api-keys", updateRequest, "superuser");

        updateResponse.ShouldBeSuccess();

        var a = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        var b = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);
        var c = await TestingServer.GetAsync("demo/apikey", apiKeyHeader);

        a.Code.ShouldBe(HttpStatusCode.OK);
        b.Code.ShouldBe(HttpStatusCode.OK);
        c.Code.ShouldBe(HttpStatusCode.OK);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
