using Demo1.Tests;

namespace rbkApiModules.Identity.Tests.Claims;

[NotInParallel(nameof(UiDefinition_Smoke_Tests))]
public class UiDefinition_Smoke_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task UiDefinitions_Returns_Success()
    {
        var response = await TestingServer.GetAsync("api/ui-definitions");

        response.ShouldBeSuccess();
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    }
}