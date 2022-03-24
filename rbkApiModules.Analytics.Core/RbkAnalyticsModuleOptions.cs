using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Analytics.Core
{
    public class RbkAnalyticsModuleOptions
    {
        public virtual double TimezoneOffsetHours { get; set; }
        public virtual double IdleInterval { get; set; }
    }
}
