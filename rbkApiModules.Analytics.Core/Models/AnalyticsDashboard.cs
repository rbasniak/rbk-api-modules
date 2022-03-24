namespace rbkApiModules.Analytics.Core
{
    public class AnalyticsDashboard
    {
        public object MostActiveDomains { get; set; }
        public object MostActiveUsers { get; set; }
        public object SlowestReadEndpoints { get; set; }
        public object MostFailedEndpoints { get; set; }
        public object MostUsedEndpoints { get; set; }
        public object BiggestResquestsEndpoints { get; set; }
        public object BiggestResponsesEndpoints { get; set; }
        public object CachedRequestsProportion { get; set; }
        public object EndpointErrorRates { get; set; }
        public object AverageTransactionsPerEndpoint { get; set; }
        public object MostResourceHungryEndpoint { get; set; }
        public object MostActiveHours { get; set; }
        public object MostActiveDays { get; set; }
        public object TotalTimeComsumptionPerReadEndpoint { get; set; }

        public object DailyActiveUsers { get; set; }
        public object DailyErrors { get; set; }
        public object DailyInboundTraffic { get; set; }
        public object DailyOutboundTraffic { get; set; }
        public object DailyAuthenticationFailures { get; set; }
        public object DailyRequests { get; set; }
        public object DailyTransactions { get; set; }
        public object DailyDatabaseUsage { get; set; }
  }

}