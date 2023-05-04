using Microsoft.EntityFrameworkCore; 

namespace Demo4;

public class TestingDatabaseContext : DatabaseContext
{
    public TestingDatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}