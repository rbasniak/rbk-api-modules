using Microsoft.EntityFrameworkCore;
using rbkApiModules.Auditing.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Auditing.SqlServer
{
    /// <summary>
    /// Store para SQL Server
    /// </summary>
    public class SqlServerAuditingStore : IAuditingModuleStore
    {
        private readonly SqlServerAuditingContext _context;
        public SqlServerAuditingStore(SqlServerAuditingContext context)
        {
            _context = context;
        }

        public void StoreData(StoredEvent data)
        {
            _context.Data.Add(data);
            _context.SaveChanges();
        }

        public async Task<List<StoredEvent>> InTimeRange(DateTime from, DateTime to)
        {
            return await _context.Data.Where(x => x.Timestamp >= from && x.Timestamp <= to).ToListAsync();
        }
    }
}
