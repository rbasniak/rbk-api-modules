using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using rbkApiModules.Identity.Core;

namespace Demo1;

public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
{
    public DatabaseContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var connectionString = "Data Source=Demo1.app"; 

        var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
        optionsBuilder
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();

        // Design-time factory doesn't have HTTP context - use null provider
        return new DatabaseContext(optionsBuilder.Options, new DesignTimeTenantProvider());
    }

    private class DesignTimeTenantProvider : ITenantProvider
    {
        public string? CurrentTenantId => null;
    }
}