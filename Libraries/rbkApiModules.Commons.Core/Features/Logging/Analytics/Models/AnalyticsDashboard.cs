using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Commons.Localization;

public class AnalyticsDashboard
{
    public object MostActiveDomains { get; set; }
    public object MostActiveUsers { get; set; }
    public object SlowestReadEndpoints { get; set; }
    public object MostFailedEndpoints { get; set; }
    public object MostUsedEndpoints { get; set; }
    public object BiggestResquestsEndpoints { get; set; }
    public object EndpointErrorRates { get; set; }
    public object AverageTransactionsPerEndpoint { get; set; }
    public object MostResourceHungryEndpoint { get; set; }
    public object MostActiveHours { get; set; }
    public object MostActiveDays { get; set; }
    public object TotalTimeComsumptionPerEndpoint { get; set; }

    public object DailyActiveUsers { get; set; }
    public object DailyErrors { get; set; }
    public object DailyRequests { get; set; }
    public object DailyTransactions { get; set; }
    public object DailyDatabaseUsage { get; set; }
}