using Demo.MultipleApis.Tests.Infrastructure;

namespace Demo.MultipleApis.Tests;

[HumanFriendlyDisplayName]
public class CrossApiIntegration_Tests
{
    [ClassDataSource<Demo1TestingServer>(Shared = SharedType.PerClass)]
    public required Demo1TestingServer Demo1Server { get; set; } = default!;

    [ClassDataSource<Demo2TestingServer>(Shared = SharedType.PerClass)]
    public required Demo2TestingServer Demo2Server { get; set; } = default!;

    [Test, NotInParallel(Order = 10)]
    public async Task Bind_Cross_Api_Clients()
    {
        var demo2Client = Demo2Server.CreateClient();
        Demo1Server.SetNamedHttpClient(nameof(Demo1.Clients.IDemo2ApiClient), demo2Client);

        var demo1Client = Demo1Server.CreateClient();
        Demo2Server.SetNamedHttpClient(nameof(Demo2.Clients.IDemo1ApiClient), demo1Client);
    }

    [Test, NotInParallel(Order = 20)]
    public async Task Demo1_Calls_Demo2_Anonymous_Endpoint()
    {
        var response = await Demo1Server.GetAsync<IntegrationResponse>("/integration/demo2/anonymous");

        response.ShouldBeSuccess(out var payload);
        payload.Source.ShouldBe("Demo1");
        payload.Sibling.ShouldBe("Demo2");
        payload.Message.ShouldBe("Anonymous");
    }

    [Test, NotInParallel(Order = 30)]
    public async Task Demo2_Calls_Demo1_Anonymous_Endpoint()
    {
        var response = await Demo2Server.GetAsync<IntegrationResponse>("/integration/demo1/anonymous");

        response.ShouldBeSuccess(out var payload);
        payload.Source.ShouldBe("Demo2");
        payload.Sibling.ShouldBe("Demo1");
        payload.Message.ShouldBe("Anonymous");
    }

    private sealed record IntegrationResponse(string Source, string Sibling, string Message);
}
