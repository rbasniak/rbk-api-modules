using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Commons
{
    public interface IDiagnosticsModuleStore
    {
        void StoreData(DiagnosticsEntry data); 
        
        Task<List<DiagnosticsEntry>> FilterAsync(DateTime from, DateTime to, string[] versions, string[] areas, string[] layers,
            string[] domains, string[] sources, string[] users, string[] browsers, string[] userAgents, string[] operatinSystems, string[] devices,
            string[] messages, string requestId);

        Task<List<DiagnosticsEntry>> AllAsync();
    }
}
