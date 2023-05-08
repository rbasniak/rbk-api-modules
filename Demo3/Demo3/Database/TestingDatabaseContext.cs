using Microsoft.EntityFrameworkCore;
using rbkApiModules.Authentication;
using rbkApiModules.Commons.Relational;
using rbkApiModules.Identity;
using rbkApiModules.Identity.Core;

namespace Demo3;

public class TestingDatabaseContext : DatabaseContext
{
    public TestingDatabaseContext(DbContextOptions<TestingDatabaseContext> options) : base(options)
    {
    }
}