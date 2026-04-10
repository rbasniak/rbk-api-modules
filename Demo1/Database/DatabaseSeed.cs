using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using rbkApiModules.Identity.Relational;
using System.Text.Json;

namespace Demo1;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    private static int _fileIndex = 1;
    private static int _folderIndex = 1;

    public DatabaseSeed()
    {
        AddSeed("2022-10-28: Platforms seed", new SeedInfo<DatabaseContext>(PlatformsSeed, EnvironmentUsage.Development | EnvironmentUsage.Testing));
    }

    public static void PlatformsSeed(DatabaseContext context, IServiceProvider serviceProvider)
    {
        var buzios = new Tenant("BUZIOS", "BUZIOS", JsonSerializer.Serialize(new { Description = "Unidade de produção da Bacia de Buzios" }));
        var unBs = new Tenant("UN-BS", "UN-BS", JsonSerializer.Serialize(new { Description = "Unidade de produção da Bacia de Santos" }));

        var p76 = new Plant(buzios.Alias, "P-76", "Plataforma P-76");
        var p77 = new Plant(buzios.Alias, "P-77", "Plataforma P-77");
        var p80 = new Plant(buzios.Alias, "P-80", "Plataforma P-80");

        var p51 = new Plant(unBs.Alias, "P-51", "Plataforma P-51");
        var p55 = new Plant(unBs.Alias, "P-55", "Plataforma P-55");

        context.Add(buzios);
        context.Add(unBs);
        context.Add(p76);
        context.Add(p77);
        context.Add(p80);
        context.Add(p51);
        context.Add(p55);

        context.SaveChanges();

        var canManageTenantRolesClaim = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_TENANT_SPECIFIC_ROLES);
        var canManageUsers = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USERS);
        var canOverrideClaims = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.OVERRIDE_USER_CLAIMS);
        var manageUSerRoles = context.Set<Claim>().First(x => x.Identification == AuthenticationClaims.MANAGE_USER_ROLES);

        var adminBuzios = new User(buzios.Alias, "admin1", "admin1@buzios.com", "123", AvatarGenerator.GenerateBase64Avatar("A 1"), "Administrador BUZIOS 1", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "911112190", "management": "ABAST/CPQ" }"""));
        adminBuzios.AddClaim(canManageTenantRolesClaim, ClaimAccessType.Allow);
        adminBuzios.AddClaim(canManageUsers, ClaimAccessType.Allow);
        adminBuzios.AddClaim(canOverrideClaims, ClaimAccessType.Allow);
        adminBuzios.AddClaim(manageUSerRoles, ClaimAccessType.Allow);
        adminBuzios.Confirm();

        var adminUnEs = new User(unBs.Alias, "admin1", "admin1@bs.com", "123", AvatarGenerator.GenerateBase64Avatar("A 1"), "Administrador UN-ES 1", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "190911112", "management": "PPR/EP" }"""));
        adminUnEs.AddClaim(canManageTenantRolesClaim, ClaimAccessType.Allow);
        adminUnEs.AddClaim(canManageUsers, ClaimAccessType.Allow);
        adminUnEs.AddClaim(canOverrideClaims, ClaimAccessType.Allow);
        adminUnEs.AddClaim(manageUSerRoles, ClaimAccessType.Allow);
        adminUnEs.Confirm();

        var superuserBuzios = new User(buzios.Alias, "superuser", "superuser@buzios.com", "123", AvatarGenerator.GenerateBase64Avatar("S U"), "Super User", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        superuserBuzios.Confirm();

        var johnDoeBuzios = new User(buzios.Alias, "john.doe", "john.doe@buzios.com", "123", AvatarGenerator.GenerateBase64Avatar("John Doe"), "John Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        johnDoeBuzios.Confirm();
        var janeDoeBuzios = new User(buzios.Alias, "jane.doe", "jane.doe@buzios.com", "123", AvatarGenerator.GenerateBase64Avatar("Jane Doe"), "Jane Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "22222222", "management": "MQT/BZ/EP" }"""));
        janeDoeBuzios.Confirm();

        var johnDoeUnBs = new User(unBs.Alias, "john.doe", "john.doe@bs.com", "abc", AvatarGenerator.GenerateBase64Avatar("John Doe"), "John Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        johnDoeUnBs.Confirm();
        var janeDoeUnBs = new User(unBs.Alias, "jane.doe", "jane.doe@bs.com", "abc", AvatarGenerator.GenerateBase64Avatar("Jane Doe"), "Jane Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        janeDoeUnBs.Confirm();
        var starkUnBs = new User(unBs.Alias, "tony.stark", "tony.stark@bs.com", "abc", AvatarGenerator.GenerateBase64Avatar("Tony Stark"), "Tony Stark", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        starkUnBs.Confirm();


        context.Add(superuserBuzios);
        context.Add(adminBuzios);
        context.Add(adminUnEs);
        context.Add(johnDoeBuzios);
        context.Add(janeDoeBuzios);
        context.Add(johnDoeUnBs);
        context.Add(janeDoeUnBs);
        context.Add(starkUnBs);

        context.SaveChanges();

        var canAproveReportsClaim = new Claim("CAN_APPROVE_REPORTS", "Can approve reports");
        canAproveReportsClaim.AllowUsageOnApiKeys();
        var canGenerateReportsClaim = new Claim("CAN_GENERATE_REPORTS", "Can generate reports");
        var canShareReports = new Claim("CAN_SHARE_REPORTS", "Can share reports");

        context.Add(canAproveReportsClaim);
        context.Add(canGenerateReportsClaim);
        context.Add(canShareReports);

        context.SaveChanges();

        ApiKeySeeding.SeedApiKeyAsync(
            context,
            "Demo integration",
            new List<Guid> { canAproveReportsClaim.Id },
            DemoIntegrationApiKey.Value,
            expirationDate: null,
            tenantId: null).GetAwaiter().GetResult();

        var generalRole = new Role("Employee");
        var generalManagerRole = new Role("Manager");
        var unbsManagerRole = new Role("UN-BS", "Manager");

        generalRole.AddClaim(canGenerateReportsClaim);

        generalManagerRole.AddClaim(canAproveReportsClaim);

        unbsManagerRole.AddClaim(canAproveReportsClaim);
        unbsManagerRole.AddClaim(canShareReports);

        context.Add(generalRole);
        context.Add(generalManagerRole);
        context.Add(unbsManagerRole);

        context.SaveChanges();

        johnDoeBuzios.AddRole(generalRole);
        johnDoeBuzios.AddClaim(canShareReports, ClaimAccessType.Allow);

        janeDoeBuzios.AddRole(generalRole);
        janeDoeBuzios.AddRole(generalManagerRole);

        johnDoeUnBs.AddRole(unbsManagerRole);

        janeDoeUnBs.AddRole(unbsManagerRole);
        janeDoeUnBs.AddClaim(canGenerateReportsClaim, ClaimAccessType.Block);
        janeDoeUnBs.AddClaim(canShareReports, ClaimAccessType.Block);

        context.SaveChanges();
    }
}
