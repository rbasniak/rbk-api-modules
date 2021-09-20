using System.Collections.Generic;
using rbkApiModules.Infrastructure.Models;
using rbkApiModules.Utilities.Charts.ChartJs;

namespace rbkApiModules.Analytics.Core
{
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
    }

}