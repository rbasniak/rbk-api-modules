using Demo1.Clients;
using Demo.MultipleApis.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.MultipleApis.Tests.Infrastructure;

public class Demo1TestingServer : MultiApiTestingServerBase<Demo1.Program>
{
    protected override string ConfigFolderName => "Demo1";

    protected override bool UseHttps => true;

    protected override Task InitializeApplicationAsync() => Task.CompletedTask;

    protected override void ConfigureTestServices(IServiceCollection services)
    {
        AddNamedHttpClient(services, nameof(IDemo2ApiClient));
    }

    protected override IEnumerable<KeyValuePair<string, string>> ConfigureInMemoryOverrides()
        => [];
}
