using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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

        // Design-time factory has no HTTP context; tenant filter returns null (all rows visible for migrations)
        return new DatabaseContext(optionsBuilder.Options, new NullHttpContextAccessor());
    }

    private class NullHttpContextAccessor : IHttpContextAccessor
    {
        public HttpContext? HttpContext { get; set; } = null;
    }
}