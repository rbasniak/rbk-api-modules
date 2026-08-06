using Demo2.Clients;
using Demo.MultipleApis.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.MultipleApis.Tests.Infrastructure;

public class Demo2TestingServer : MultiApiTestingServerBase<Demo2.Program>
{
    protected override string ConfigFolderName => "Demo2";

    protected override bool UseHttps => true;

    protected override Task InitializeApplicationAsync() => Task.CompletedTask;

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        AddNamedHttpClient(services, nameof(IDemo1ApiClient));
    }

    protected override IEnumerable<KeyValuePair<string, string>> ConfigureInMemoryOverrides()
        => [];
}
