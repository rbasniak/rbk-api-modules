using Microsoft.EntityFrameworkCore;
using rbkApiModules.Diagnostics.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Relational
{
    /// <summary>
    /// Store para bancos relacionais
    /// </summary>
    public class RelationalDiagnosticsStore : IDiagnosticsModuleStore
    {
        private readonly BaseDiagnosticsContext _context;
        public RelationalDiagnosticsStore(BaseDiagnosticsContext context)
        {
            _context = context;
        }

        public void StoreData(DiagnosticsEntry request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        }

        public async Task<List<DiagnosticsEntry>> FilterAsync(DateTime from, DateTime to)
        {
            return await FilterAsync(from, to, null, null, null, null, null, null, null, null, null, null, null, null);
        }

        public async Task<List<DiagnosticsEntry>> FilterAsync(DateTime from, DateTime to, string[] versions, string[] areas, 
            string[] layers, string[] domains, string[] sources, string[] users, string[] browsers, string[] agents, 
            string[] operatinSystems, string[] devices, string[] messages, string requestId)
        {
            var query = _context.Data
                .Where(x => x.Timestamp >= from && x.Timestamp <= to);

            if (versions != null && versions.Length > 0)
            {
                query = query.Where(x => versions.Any(version => x.ApplicationVersion == version));
                var temp = query.ToList();
            }

            if (areas != null && areas.Length > 0)
            {
                query = query.Where(x => areas.Any(area => x.ApplicationArea == area));
                var temp = query.ToList();
            }

            if (layers != null && layers.Length > 0)
            {
                query = query.Where(x => layers.Any(layer => x.ApplicationLayer == layer));
                var temp = query.ToList();
            }

            if (domains != null && domains.Length > 0)
            {
                query = query.Where(x => domains.Any(domain => x.Domain == domain));
                var temp = query.ToList();
            }

            if (sources != null && sources.Length > 0)
            {
                query = query.Where(x => sources.Any(source => x.ExceptionSource == source));
                var temp = query.ToList();
            }

            if (users != null && users.Length > 0)
            {
                query = query.Where(x => users.Any(user => x.Username == user));
                var temp = query.ToList();
            }

            if (browsers != null && browsers.Length > 0)
            {
                query = query.Where(x => browsers.Any(browser => x.ClientBrowser == browser));
                var temp = query.ToList();
            }

            if (agents != null && agents.Length > 0)
            {
                query = query.Where(x => agents.Any(agent => x.ClientUserAgent == agent));
                var temp = query.ToList();
            }

            if (devices != null && devices.Length > 0)
            {
                query = query.Where(x => devices.Any(device => x.ClientDevice == device));
                var temp = query.ToList();
            }

            if (operatinSystems != null && operatinSystems.Length > 0)
            {
                query = query.Where(x => operatinSystems.Any(os => x.ClientOperatingSystem + " " + x.ClientOperatingSystemVersion == os));
                var temp = query.ToList();
            }

            if (devices != null && devices.Length > 0)
            {
                query = query.Where(x => devices.Any(device => x.ClientDevice == device));
                var temp = query.ToList();
            }

            if (messages != null && messages.Length > 0)
            {
                query = query.Where(x => messages.Any(message => x.ExceptionMessage == message));
                var temp = query.ToList();
            }

            if (!String.IsNullOrEmpty(requestId))
            {
                query = query.Where(x => EF.Functions.Like(x.RequestId, requestId));
            }

            var results = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return results;
        } 

        public async Task<List<DiagnosticsEntry>> AllAsync()
        {
            return await _context.Data.ToListAsync();
        }

        public void DeleteOldEntries(int daysToKeep)
        {
            var limit = DateTime.UtcNow.AddDays(-daysToKeep);

            var results = _context.Data.Where(x => x.Timestamp < limit).ToList();

            _context.RemoveRange(results);

            _context.SaveChanges();
        }
    }
}
