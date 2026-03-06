using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Demo2.Tests;

public class Demo2TestingServer : RbkTestingServer<Demo2.Program>
{
    protected override bool UseHttps => true;

    protected override Task InitializeApplicationAsync()
    {
        return Task.CompletedTask;
    }

    protected override void ConfigureAppConfiguration(WebHostBuilderContext context, IConfigurationBuilder config)
    {
    }

    protected override void ConfigureTestServices(IServiceCollection services)
    {
    }

    protected override IEnumerable<KeyValuePair<string, string>> ConfigureInMemoryOverrides()
    {
        return Array.Empty<KeyValuePair<string, string>>();
    }
}
