using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rbkApiModules.Logs.Core
{
    public interface ILogsModuleStore
    {
        void StoreData(LogEntry data);

        void StoreData(string applicationLayer, string applicationArea, string applicationVersion, string source, string message, LogLevel level,
            object input = null, string username = null, string domain = null);

        void StoreData(HttpContext context, string source, string message, LogLevel level, object input = null);

        void StoreData(LogApplicationInfo logApplicationInfo, string source, string message, LogLevel level,
            object input = null, string username = null, string domain = null);

        Task<List<LogEntry>> FilterAsync(DateTime from, DateTime to);

        Task<List<LogEntry>> FilterAsync(DateTime from, DateTime to, string[] messages, LogLevel[] levels, string[] layers, string[] areas, string[] versions,
            string[] sources, string[] enviroments, string[] enviromentsVersions, string[] users, string[] domains, string[] machines);

        Task<List<LogEntry>> AllAsync();

        void DeleteOldEntries(int daysToKeep);
    }
}
