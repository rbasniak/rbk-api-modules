using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using rbkApiModules.Diagnostics.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.SqlServer
{
    /// <summary>
    /// Store para SQL Server
    /// </summary>
    public class SqlServerDiagnosticsStore : IDiagnosticsModuleStore
    {
        private readonly SqlServerDiagnosticsContext _context;
        public SqlServerDiagnosticsStore(SqlServerDiagnosticsContext context)
        {
            _context = context;
        }

        public void StoreData(DiagnosticsEntry request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        }

        public async Task<List<DiagnosticsEntry>> FilterAsync(DateTime from, DateTime to, string[] versions, string[] areas, 
            string[] layers, string[] domains, string[] sources, string[] users, string[] browsers, string[] agents, 
            string[] operatinSystems, string[] devices, string messageContains, string requestId)
        {
            var query = _context.Data
                .Where(x => x.Timestamp >= from && x.Timestamp <= to);

            if (versions != null && versions.Length > 0)
            {
                query = query.Where(x => versions.Any(version => x.ApplicationVersion == version));
            }

            if (areas != null && areas.Length > 0)
            {
                query = query.Where(x => areas.Any(area => x.ApplicationArea == area));
            }

            if (layers != null && layers.Length > 0)
            {
                query = query.Where(x => layers.Any(layer => x.ApplicationLayer == layer));
            }

            if (domains != null && domains.Length > 0)
            {
                query = query.Where(x => domains.Any(domain => x.Domain == domain));
            }

            if (sources != null && sources.Length > 0)
            {
                query = query.Where(x => sources.Any(action => x.ExceptionSource == action));
            }

            if (users != null && users.Length > 0)
            {
                query = query.Where(x => users.Any(user => x.Username == user));
            }

            if (browsers != null && browsers.Length > 0)
            {
                query = query.Where(x => agents.Any(browser => x.ClientBrowser == browser));
            }

            if (agents != null && agents.Length > 0)
            {
                query = query.Where(x => agents.Any(agent => x.ClientUserAgent == agent));
            }

            if (devices != null && devices.Length > 0)
            {
                query = query.Where(x => devices.Any(device => x.ClientDevice == device));
            }

            if (operatinSystems != null && operatinSystems.Length > 0)
            {
                query = query.Where(x => operatinSystems.Any(os => x.ClientOperatingSystem + " " + x.ClientOperatingSystemVersion  == os));
            }

            if (!String.IsNullOrEmpty(messageContains))
            {
                query = query.Where(x => EF.Functions.Like(x.ExceptionMessage, $"%{messageContains}%"));
            }

            if (!String.IsNullOrEmpty(requestId))
            {
                query = query.Where(x => EF.Functions.Like(x.RequestId, requestId));
            }

            var result = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return result;
        }

        public async Task<List<DiagnosticsEntry>> AllAsync()
        {
            return await _context.Data.ToListAsync();
        }
    }
}
