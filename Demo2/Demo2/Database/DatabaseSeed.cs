using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Demo2.Domain.Events.MyImplementation.Database;

public class DatabaseSeed : DatabaseSeedManager<DatabaseContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("2023-02-07: Initial seed", new SeedInfo<DatabaseContext>(context => InitialSeed(context), useInProduction: true));
    }

    public static void InitialSeed(DatabaseContext context)
    {
        
    }
}