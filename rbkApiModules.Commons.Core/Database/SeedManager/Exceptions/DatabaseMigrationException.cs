namespace rbkApiModules.Commons.Relational;

public class DatabaseMigrationException : Exception
{
    public DatabaseMigrationException(Exception exception) : base("Error while migrating the database", exception)
    {
    }
}
