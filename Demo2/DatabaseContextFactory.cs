using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using rbkApiModules.Identity.Core;

namespace Demo2;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder
            .UseSqlite("Data Source=Demo2.app")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        // Design-time factory has no HTTP context; tenant filter returns null (all rows visible for migrations)
        return new DatabaseContext(optionsBuilder.Options, new DesignTimeTenantProvider());
    }

    private class DesignTimeTenantProvider : ITenantProvider
    {
        public string? CurrentTenantId => null;
    }
}
