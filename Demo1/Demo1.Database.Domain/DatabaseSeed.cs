using System.Text.Json;
using Demo1.Models.Domain;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;
using Demo1.Models.Domain.Folders;

namespace Demo1.Database.Domain;

public class DatabaseSeed: DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    private static int _fileIndex = 1;
    private static int _folderIndex = 1;

    public DatabaseSeed()
    {
        AddSeed("2022-10-28: Platforms seed", new SeedInfo<DatabaseContext>(context => PlatformsSeed(context), useInProduction: true));
        // AddSeed("2023-01-26: Folders seed", new SeedInfo<DatabaseContext>(context => FoldersSeed(context), useInProduction: true));
    }

    public static void PlatformsSeed(DatabaseContext context)
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

        var adminBuzios = new User(buzios.Alias, "admin1", "admin1@buzios.com", "123", "", "Administrador BUZIOS 1", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "911112190", "management": "ABAST/CPQ" }"""));
        adminBuzios.AddClaim(canManageTenantRolesClaim, ClaimAccessType.Allow);
        adminBuzios.AddClaim(canManageUsers, ClaimAccessType.Allow);
        adminBuzios.AddClaim(canOverrideClaims, ClaimAccessType.Allow);
        adminBuzios.AddClaim(manageUSerRoles, ClaimAccessType.Allow);
        adminBuzios.Confirm();

        var adminUnEs = new User(unBs.Alias, "admin1", "admin1@bs.com", "123", "", "Administrador UN-ES 1", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "190911112", "management": "PPR/EP" }"""));
        adminUnEs.AddClaim(canManageTenantRolesClaim, ClaimAccessType.Allow);
        adminUnEs.AddClaim(canManageUsers, ClaimAccessType.Allow);
        adminUnEs.AddClaim(canOverrideClaims, ClaimAccessType.Allow);
        adminUnEs.AddClaim(manageUSerRoles, ClaimAccessType.Allow);
        adminUnEs.Confirm();

        var johnDoeBuzios = new User(buzios.Alias, "john.doe", "john.doe@buzios.com", "123", "", "John Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        johnDoeBuzios.Confirm();
        var janeDoeBuzios = new User(buzios.Alias, "jane.doe", "jane.doe@buzios.com", "123", "", "Jane Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "22222222", "management": "MQT/BZ/EP" }"""));
        janeDoeBuzios.Confirm();

        var johnDoeUnBs = new User(unBs.Alias, "john.doe", "john.doe@bs.com", "abc", "", "John Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        johnDoeUnBs.Confirm();        
        var janeDoeUnBs = new User(unBs.Alias, "jane.doe", "jane.doe@bs.com", "abc", "", "Jane Doe", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        janeDoeUnBs.Confirm();
        var starkUnBs = new User(unBs.Alias, "tony.stark", "tony.stark@bs.com", "abc", "", "Tony Stark", AuthenticationMode.Credentials, JsonSerializer.Deserialize<Dictionary<string, string>>("""{ "phone": "11111111", "management": "MQT/ES/EG" }"""));
        starkUnBs.Confirm();


        context.Add(adminBuzios);
        context.Add(adminUnEs);
        context.Add(johnDoeBuzios);
        context.Add(janeDoeBuzios);
        context.Add(johnDoeUnBs);
        context.Add(janeDoeUnBs);
        context.Add(starkUnBs);

        context.SaveChanges();

        var canAproveReportsClaim = new Claim("CAN_APPROVE_REPORTS", "Can approve reports");
        var canGenerateReportsClaim = new Claim("CAN_GENERATE_REPORTS", "Can generate reports");
        var canShareReports = new Claim("CAN_SHARE_REPORTS", "Can share reports");

        context.Add(canAproveReportsClaim);
        context.Add(canGenerateReportsClaim);

        context.SaveChanges();

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

    public static void FoldersSeed(DatabaseContext context)
    {
        var random = new Random(19001);

        for (int i = 0; i < 1000; i++)
        {
            var folder = new Folder($"Folder {_folderIndex}", $"This is the awesome description for the folder {_folderIndex}");
            _folderIndex++;
            var depth = 0;
            PopulateFolderWithFiles(folder, random, depth);
            context.Add(folder);
        }

        context.SaveChanges();
    }

    private static void PopulateFolderWithFiles(Folder folder, Random random, int depth)
    {
        depth = depth + 1;
        if (random.NextDouble() > 0.4) 
        {
            for (int i = 0; i < random.Next(0, 1000); i++)
            {
                folder.AddFile($"File {_fileIndex}", $"This is the awesome description for the file {_fileIndex}", ".dat", random.Next(100, 1000));
                _fileIndex++;
            }
        }

        if (random.NextDouble() > 0.8 && depth <= 3)
        {
            for (int i = 0; i < random.Next(0, 100); i++)
            {
                var child = folder.AddChild($"Folder {_folderIndex}", $"This is the awesome description for the folder {_folderIndex}");
                _folderIndex++;

                PopulateFolderWithFiles(child, random, depth);
            }
        }
    }
}
 