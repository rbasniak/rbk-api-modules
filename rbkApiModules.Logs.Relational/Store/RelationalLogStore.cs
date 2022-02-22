using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using rbkApiModules.Logs.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace rbkApiModules.Logs.Relational
{
    /// <summary>
    /// Store para para bancos relacionais
    /// </summary>
    public class RelationalLogStore : ILogsModuleStore
    {
        private readonly BaseLogContext _context;
        public RelationalLogStore(BaseLogContext context)
        {
            _context = context;
        }

        public void StoreData(LogEntry request)
        {
            _context.Data.Add(request);
            _context.SaveChanges();
        }

        public void StoreData(string applicationLayer, string applicationArea, string applicationVersion, string source, string message, LogLevel level,
            object input = null, string username = null, string domain = null)
        {
            var entry = new LogEntry
            {
                ApplicationLayer = applicationLayer,
                ApplicationArea = applicationArea,
                ApplicationVersion = applicationVersion,
                InputData = JsonConvert.SerializeObject(input, Formatting.Indented),
                Source = source,
                Enviroment = Environment.OSVersion.Platform.ToString(),
                EnviromentVersion = Environment.OSVersion.VersionString,
                MachineName = Environment.MachineName,
                Message = message,
                Level = level,
                Username = username,
                Domain = domain
            };

            _context.Data.Add(entry);
            _context.SaveChanges();
        }

        public void StoreData(HttpContext context, string source, string message, LogLevel level, object input = null)
        {
            var area = context.Items.FirstOrDefault(x => x.Key.ToString() == LogEntry.LOG_ENTRY_AREA);
            var user = (System.Security.Claims.ClaimsIdentity)context.User.Identity;
            var username = user.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            var domain = user.Claims.FirstOrDefault(c => c.Type == "domain")?.Value;

            StoreData("API", area.Key != null ? area.Value as string : string.Empty, "1.0.0", // TODO: pegar de algum lugar
                source, message, level, input, username, domain);
        }

        public void StoreData(LogApplicationInfo logApplicationInfo, string source, string message, LogLevel level,
            object input = null, string username = null, string domain = null)
        {
            StoreData(logApplicationInfo.ApplicationLayer, logApplicationInfo.ApplicationArea, logApplicationInfo.ApplicationVersion,
                source, message, level, input, username, domain);
        }

        public async Task<List<LogEntry>> FilterAsync(DateTime from, DateTime to)
        {
            return await FilterAsync(from, to, null, null, null, null, null, null, null, null, null, null, null);
        }

        public async Task<List<LogEntry>> FilterAsync(DateTime from, DateTime to, string[] messages, LogLevel[] levels, string[] layers, string[] areas, string[] versions,
            string[] sources, string[] enviroments, string[] enviromentsVersions, string[] users, string[] domains, string[] machines)
        {
            var query = _context.Data
                .Where(x => x.Timestamp >= from && x.Timestamp <= to);

            if (messages != null && messages.Length > 0)
            {
                query = query.Where(x => messages.Any(message => x.Message == message));
                var temp = query.ToList();
            }

            if (levels != null && levels.Length > 0)
            {
                query = query.Where(x => levels.Any(level => x.Level == level));
                var temp = query.ToList();
            }

            if (layers != null && layers.Length > 0)
            {
                query = query.Where(x => layers.Any(layer => x.ApplicationLayer == layer));
                var temp = query.ToList();
            }

            if (areas != null && areas.Length > 0)
            {
                query = query.Where(x => areas.Any(area => x.ApplicationArea == area));
                var temp = query.ToList();
            }

            if (versions != null && versions.Length > 0)
            {
                query = query.Where(x => versions.Any(version => x.ApplicationVersion == version));
                var temp = query.ToList();
            }

            if (sources != null && sources.Length > 0)
            {
                query = query.Where(x => sources.Any(source => x.Source == source));
                var temp = query.ToList();
            }

            if (enviroments != null && enviroments.Length > 0)
            {
                query = query.Where(x => enviroments.Any(enviroment => x.Enviroment == enviroment));
                var temp = query.ToList();
            }

            if (enviromentsVersions != null && enviromentsVersions.Length > 0)
            {
                query = query.Where(x => enviromentsVersions.Any(enviromentVersion => x.EnviromentVersion == enviromentVersion));
                var temp = query.ToList();
            }

            if (users != null && users.Length > 0)
            {
                query = query.Where(x => users.Any(user => x.Username == user));
                var temp = query.ToList();
            }

            if (domains != null && domains.Length > 0)
            {
                query = query.Where(x => domains.Any(domain => x.Domain == domain));
                var temp = query.ToList();
            }

            if (machines != null && machines.Length > 0)
            {
                query = query.Where(x => machines.Any(machine => x.MachineName == machine));
                var temp = query.ToList();
            }

            var results = await query.OrderByDescending(x => x.Timestamp).ToListAsync();

            return results;
        } 

        public async Task<List<LogEntry>> AllAsync()
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
