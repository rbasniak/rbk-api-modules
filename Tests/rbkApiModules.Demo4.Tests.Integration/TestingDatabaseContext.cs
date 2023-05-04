using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Identity.Core;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity;

namespace Demo4;

public class TestingDatabaseContext : DatabaseContext
{
    public TestingDatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=my.db");
}