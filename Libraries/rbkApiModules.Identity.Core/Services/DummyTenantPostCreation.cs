namespace rbkApiModules.Identity.Core;

public class DummyTenantPostCreationActions : ITenantPostCreationAction
{
    public Task ExecuteAsync(Tenant tenant, CancellationToken cancellation = default)
    {
        return Task.CompletedTask;
    }
}
