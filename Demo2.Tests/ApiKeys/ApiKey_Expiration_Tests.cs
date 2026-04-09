using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_Expiration_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    [Test, NotInParallel(Order = 2)]
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

    [Test, NotInParallel(Order = 3)]
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

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
