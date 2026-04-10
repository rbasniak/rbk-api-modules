using Demo2;
using Demo2.Tests;
using rbkApiModules.Identity.Core;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_UsageStatistics_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

    [Test, NotInParallel(Order = 2)]
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

    [Test, NotInParallel(Order = 3)]
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

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
