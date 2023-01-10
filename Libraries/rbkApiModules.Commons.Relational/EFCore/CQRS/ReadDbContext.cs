using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Commons.Relational.CQRS;

public abstract class ReadDbContext : DbContext
{
    public ReadDbContext() : base()
    {

    }

    public ReadDbContext(DbContextOptions options) : base(options)
    {

    }
}
