using System.Collections.Generic;
using rbkApiModules.Infrastructure.Models;

namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsResults
    {
        public AnalyticsResults()
        {
            MostActiveDomains = new List<SimpleLabeledValue<int>>();
            MostActiveUsers = new List<SimpleLabeledValue<int>>();
            SlowestReadEndpoints = new List<SimpleLabeledValue<int>>();
            SlowestWriteEndpoints = new List<SimpleLabeledValue<int>>();
            MostFailedEndpoints = new List<SimpleLabeledValue<int>>();
            MostUsedEndpoints = new List<SimpleLabeledValue<int>>();
            BiggestResquestsEndpoints = new List<SimpleLabeledValue<int>>();
            BiggestResponsesEndpoints = new List<SimpleLabeledValue<int>>();
            CachedRequestsProportion = new List<SimpleLabeledValue<double>>();
            EndpointErrorRates = new List<SimpleLabeledValue<double>>();

            AverageTransactionsPerEndpoint = new List<SimpleLabeledValue<int>>();
            MostResourceHungryEndpoint = new List<SimpleLabeledValue<int>>();
            MostActiveHours = new List<SimpleLabeledValue<int>>();
            MostActiveDays = new List<SimpleLabeledValue<int>>();
            TotalTimeComsumptionPerReadEndpoint = new List<SimpleLabeledValue<int>>();
            TotalTimeComsumptionPerWriteEndpoint = new List<SimpleLabeledValue<int>>();

            DailyActiveUsers = new List<DateValuePoint>();
            DailyErrors = new List<DateValuePoint>();
            DailyInboundTraffic = new List<DateValuePoint>();
            DailyOutboundTraffic = new List<DateValuePoint>();
            DailyAuthenticationFailures = new List<DateValuePoint>();
            DailyRequests = new List<DateValuePoint>();
            DailyTransactions = new List<DateValuePoint>();
            DailyDatabaseUsage = new List<DateValuePoint>();
        }

        public List<SimpleLabeledValue<int>> MostActiveDomains { get; set; }
        public List<SimpleLabeledValue<int>> MostActiveUsers { get; set; }
        public List<SimpleLabeledValue<int>> SlowestReadEndpoints { get; set; }
        public List<SimpleLabeledValue<int>> SlowestWriteEndpoints { get; set; }
        public List<SimpleLabeledValue<int>> MostFailedEndpoints { get; set; }
        public List<SimpleLabeledValue<int>> MostUsedEndpoints { get; set; }
        public List<SimpleLabeledValue<int>> BiggestResquestsEndpoints { get; set; }
        public List<SimpleLabeledValue<int>> BiggestResponsesEndpoints { get; set; }
        public List<SimpleLabeledValue<double>> CachedRequestsProportion { get; set; }
        public List<SimpleLabeledValue<double>> EndpointErrorRates { get; set; }
        public List<SimpleLabeledValue<int>> AverageTransactionsPerEndpoint { get; set; }
        public List<SimpleLabeledValue<int>> MostResourceHungryEndpoint { get; set; }
        public List<SimpleLabeledValue<int>> MostActiveHours { get; set; }
        public List<SimpleLabeledValue<int>> MostActiveDays { get; set; }
        public List<SimpleLabeledValue<int>> TotalTimeComsumptionPerReadEndpoint { get; set; }
        public List<SimpleLabeledValue<int>> TotalTimeComsumptionPerWriteEndpoint { get; set; }

        public List<DateValuePoint> DailyActiveUsers { get; set; }
        public List<DateValuePoint> DailyErrors { get; set; }
        public List<DateValuePoint> DailyInboundTraffic { get; set; }
        public List<DateValuePoint> DailyOutboundTraffic { get; set; }
        public List<DateValuePoint> DailyAuthenticationFailures { get; set; }
        public List<DateValuePoint> DailyRequests { get; set; }
        public List<DateValuePoint> DailyTransactions { get; set; }
        public List<DateValuePoint> DailyDatabaseUsage { get; set; }
  }

}