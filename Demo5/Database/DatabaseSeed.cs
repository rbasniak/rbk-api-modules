using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace Demo5;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("2023-03-24: Users seed", new SeedInfo<DatabaseContext>((context, serviceProvider) => UsersSeed(context, serviceProvider), EnvironmentUsage.Development | EnvironmentUsage.Testing));
    }

    public static void UsersSeed(DatabaseContext context, IServiceProvider serviceProvider)
    {
        context.Add(new Tenant("DEMO", "Demo Company"));

        var user1 = context.Add(new User("DEMO", "user1", "user1@dem.com", "password1", null, "User1", AuthenticationMode.Credentials)).Entity;
        user1.Confirm();

        var user2 = context.Add(new User("DEMO", "user2", "user2@dem.com", "password2", null, "User2", AuthenticationMode.Credentials)).Entity;
        user2.Confirm();

        var claim = context.Add(new Claim("RESOURCE::READ", "Can read applications resources")).Entity;
        user2.AddClaim(claim, ClaimAccessType.Allow);

        context.SaveChanges();
    }
}
