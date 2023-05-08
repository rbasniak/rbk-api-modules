using Microsoft.EntityFrameworkCore; 

namespace Demo4;

public class TestingDatabaseContext : DatabaseContext
{
    public TestingDatabaseContext(DbContextOptions<TestingDatabaseContext> options) : base(options)
    {
    }
}