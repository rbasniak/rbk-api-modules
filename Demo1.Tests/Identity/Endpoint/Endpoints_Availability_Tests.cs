using Demo1.Tests;
using rbkApiModules.Identity.Core;

namespace rbkApiModules.Identity.Tests.Claims;

[NotInParallel(nameof(Endpoints_Availability_Tests))]
public class Endpoints_Availability_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer TestingServer { get; set; } = default!;

    [Test, NotInParallel(Order = 1)]
    public async Task Seed()
    {
        await Task.CompletedTask;
    }

    [Test, NotInParallel(Order = 2)]
    public async Task Switch_domain_endpoint_should_not_be_available()
    {
        // Prepare
        var request = new SwitchTenant.Request();

        // Act
        var response = await TestingServer.PostAsync<JwtResponse>("api/authentication/switch-domain", request);

        // Assert
        response.ShouldHaveErrors(HttpStatusCode.NotFound);
    }

    [Test, NotInParallel(Order = 99)]
    public async Task CleanUp()
    {
        await TestingServer.CreateContext().Database.EnsureDeletedAsync();
    } 
}