using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace Demo4;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("2023-05-04: Users seed", new SeedInfo<DatabaseContext>((context, serviceProvider) => UsersSeed(context, serviceProvider), useInProduction: true));
    }

    public static void UsersSeed(DatabaseContext context, IServiceProvider serviceProvider)
    {
        var tenant1 = context.Add(new Tenant("PARKER INDUSTRIES", "Parker Industries")).Entity;
        var tenant2 = context.Add(new Tenant("OSCORP INDUSTRIES", "Oscorp Industries")).Entity;

        var user1 = context.Add(new User("PARKER INDUSTRIES", Environment.UserName, "admin@parker-industries.com", null, null, "John Doe", AuthenticationMode.Windows, new Dictionary<string, string>
        {
            { "Sector", "Administration" },
            { "IsManager", "false" }
        })).Entity;
        user1.Confirm();
        user1.AddClaim(context.Claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);

        var user2 = context.Add(new User("OSCORP INDUSTRIES", Environment.UserName, "admin@oscorp-industries.com", null, null, "John Doe", AuthenticationMode.Windows, new Dictionary<string, string>
        {
            { "Sector", "Research" },
            { "IsManager", "true" }
        })).Entity;
        user2.Confirm();

        context.SaveChanges();

        var temp = context.Set<User>().ToList();
    }
}
