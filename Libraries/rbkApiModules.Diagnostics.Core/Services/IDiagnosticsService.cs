using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Diagnostics.Core;


public interface IDiagnostricsService
{
    Task<DiagnosticsEntry[]> GetAllAsync(DateTime startDate, DateTime endDate, CancellationToken cancellation); 
}
