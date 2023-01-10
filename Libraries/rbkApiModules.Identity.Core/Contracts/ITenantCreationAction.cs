namespace rbkApiModules.Identity.Core;

public interface ITenantPostCreationAction
{
    Task ExecuteAsync(Tenant tenant, CancellationToken cancellation = default);
}
