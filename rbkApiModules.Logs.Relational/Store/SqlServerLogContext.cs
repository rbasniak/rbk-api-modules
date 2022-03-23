using Microsoft.EntityFrameworkCore;

namespace rbkApiModules.Logs.Relational
{
    /// <summary>
    /// DBContext para a store do banco Sql Server
    /// </summary>
    public class SqlServerLogContext : BaseLogContext
    {
        public SqlServerLogContext(DbContextOptions<SqlServerLogContext> options)
            : base(options)
        {
        }
    }
}
