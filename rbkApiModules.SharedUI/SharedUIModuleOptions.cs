using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.SharedUI
{
    public class SharedUIModuleOptions
    {
        private bool _useAnalytics;
        private bool _useDiagnostics;

        public SharedUIModuleOptions()
        {
        }

        public SharedUIModuleOptions UseAnalytics()
        {
            _useAnalytics = true;
            return this;
        }

        public SharedUIModuleOptions UseDiagnostics()
        {
            _useDiagnostics = true;
            return this;
        }

        public bool UsingAnalyticsModule => _useAnalytics;

        public bool UsingDiagnosticsModule => _useDiagnostics;

    }
}
