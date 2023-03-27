﻿using System.Text.Json;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;

namespace Demo3;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("2023-03-24: Users seed", new SeedInfo<DatabaseContext>(context => UsersSeed(context), useInProduction: true));
    }

    public static void UsersSeed(DatabaseContext context)
    {
        var tenant = context.Add(new Tenant("WAYNE INC", "Wayne Enterprises")).Entity;

        var user = context.Add(new User("WAYNE INC", Environment.UserName, "admin@wayne-inc.com", "dumb_password", null, "John Doe", new Dictionary<string, string>
        {
            { "Sector", "Administration" },
            { "IsManager", "false" }
        })).Entity;
        user.Confirm();
        user.AddClaim(context.Claims.First(x => x.Identification == AuthenticationClaims.MANAGE_USERS), ClaimAccessType.Allow);
         
        context.SaveChanges();

        var temp = context.Set<User>().ToList();
    }
}
