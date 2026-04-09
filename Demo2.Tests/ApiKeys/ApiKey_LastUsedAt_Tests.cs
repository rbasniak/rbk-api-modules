using Demo2;
using Demo2.Tests;
using ApiKey = rbkApiModules.Commons.Testing.ApiKey;
using EntityApiKey = rbkApiModules.Identity.Core.ApiKey;

namespace rbkApiModules.Identity.Tests.ApiKeys;

[HumanFriendlyDisplayName]
public class ApiKey_LastUsedAt_Tests
{
    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await TestingServer.CacheCredentialsAsync("superuser", "admin", null);
    }

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

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}
