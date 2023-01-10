using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational.CQRS;

public abstract class WriteDbContext : DbContext
{
    public WriteDbContext() : base()
    {

    }

    public WriteDbContext(DbContextOptions options) : base(options)
    {

    }
}
