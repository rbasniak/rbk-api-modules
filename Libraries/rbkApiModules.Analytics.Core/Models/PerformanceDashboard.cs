﻿namespace rbkApiModules.Commons.Analytics;

public class PerformanceDashboard
{
    public object DurationDistribution { get; set; }
    public object DurationEvolution { get; set; }
    public object InSizeDistribution { get; set; }
    public object InSizeEvolution { get; set; }
    public object OutSizeDistribution { get; set; }
    public object OutSizeEvolution { get; set; }
    public object TransactionCountDistribution { get; set; }
    public object TransactionCountEvolution { get; set; }
    public object DatabaseDurationDistribution { get; set; }
    public object DatabaseDurationEvolution { get; set; }
    public object DailyUsage { get; set; }
    public object UserUsage { get; set; }
}