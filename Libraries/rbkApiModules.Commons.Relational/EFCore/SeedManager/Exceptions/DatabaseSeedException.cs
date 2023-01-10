namespace rbkApiModules.Commons.Relational;

public class DatabaseSeedException : Exception
{
    public DatabaseSeedException(Exception exception, string seedPhase) : base("Error while seeding the database", exception)
    {
        SeedPhase = seedPhase;
    }

    public string SeedPhase { get; }
}