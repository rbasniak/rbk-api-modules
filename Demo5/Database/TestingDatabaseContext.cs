using Microsoft.EntityFrameworkCore; 

namespace Demo5;

public class TestingDatabaseContext : DatabaseContext
{
    public TestingDatabaseContext(DbContextOptions<TestingDatabaseContext> options) : base(options)
    {
    }
}