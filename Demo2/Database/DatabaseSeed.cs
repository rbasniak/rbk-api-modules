using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;
using System.Text.Json;

namespace Demo2;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("Demo2: Identity seed", new SeedInfo<DatabaseContext>(IdentitySeed, EnvironmentUsage.Development | EnvironmentUsage.Testing));
    }

    public static void IdentitySeed(DatabaseContext context, IServiceProvider serviceProvider)
    {
        var buzios = new Tenant("BUZIOS", "BUZIOS", JsonSerializer.Serialize(new { Description = "Demo2 Buzios" }));
        var unBs = new Tenant("UN-BS", "UN-BS", JsonSerializer.Serialize(new { Description = "Demo2 UN-BS" }));

        context.Add(buzios);
        context.Add(unBs);
        context.SaveChanges();

        var canManageUsers = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);
        var manageApiKeysClaim = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.CAN_MANAGE_APIKEYS);

        var adminBuzios = new User(buzios.Alias, "admin1", "admin1@buzios.com", "", AvatarGenerator.GenerateBase64Avatar("A 1"), "Administrador BUZIOS 1", AuthenticationMode.Windows, null);
        adminBuzios.AddClaim(canManageUsers, ClaimAccessType.Allow);
        adminBuzios.AddClaim(manageApiKeysClaim, ClaimAccessType.Allow);
        adminBuzios.Confirm();

        var johnDoeBuzios = new User(buzios.Alias, "john.doe", "john.doe@buzios.com", "", AvatarGenerator.GenerateBase64Avatar("John Doe"), "John Doe", AuthenticationMode.Windows, null);
        johnDoeBuzios.Confirm();

        context.Add(adminBuzios);
        context.Add(johnDoeBuzios);
        context.SaveChanges();

        var demoApiClaim = new Claim("DEMO2_INTEGRATION", "Demo2 API integration");
        demoApiClaim.AllowUsageOnApiKeys();
        context.Add(demoApiClaim);
        context.SaveChanges();

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo2 integration",
            new List<Guid> { demoApiClaim.Id },
            DemoIntegrationApiKey.Value,
            expirationDate: null,
            tenantId: null).GetAwaiter().GetResult();

        var crossTenantApiKeysClaim = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.CAN_CREATE_CROSS_TENANT_API_KEYS);

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo2 global manage+cross",
            new List<Guid> { manageApiKeysClaim.Id, crossTenantApiKeysClaim.Id, demoApiClaim.Id },
            Demo2TestApiKeys.GlobalManageAndCrossTenant,
            expirationDate: null,
            tenantId: null).GetAwaiter().GetResult();

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo2 global manage only",
            new List<Guid> { manageApiKeysClaim.Id, demoApiClaim.Id },
            Demo2TestApiKeys.GlobalManageOnly,
            expirationDate: null,
            tenantId: null).GetAwaiter().GetResult();

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo2 buzios manage",
            new List<Guid> { manageApiKeysClaim.Id, demoApiClaim.Id },
            Demo2TestApiKeys.BuziosManage,
            expirationDate: null,
            tenantId: buzios.Alias).GetAwaiter().GetResult();

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo2 un-bs manage",
            new List<Guid> { manageApiKeysClaim.Id, demoApiClaim.Id },
            Demo2TestApiKeys.UnBsManage,
            expirationDate: null,
            tenantId: unBs.Alias).GetAwaiter().GetResult();
    }
}
