﻿using rbkApiModules.Commons.Relational;

namespace Demo2.Samples.Eventsourcing.EventOrientedChanges.Database;

public class DatabaseSeed : DatabaseSeedManager<EventSourcingContext>, IDatabaseSeeder
{
    public DatabaseSeed()
    {
        AddSeed("2023-02-07: Initial seed", new SeedInfo<EventSourcingContext>(context => InitialSeed(context), useInProduction: true));
    }

    public static void InitialSeed(EventSourcingContext context)
    {

    }
}