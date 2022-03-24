using System.Collections.Generic;

namespace rbkApiModules.SharedUI
{
    public class SharedUIModuleOptions
    {
        internal static bool _useAnalytics;
        internal static bool _useDiagnostics;
        internal static List<CustomRoute> _customRoutes = new List<CustomRoute>();

        internal SharedUIModuleOptions()
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

        public SharedUIModuleOptions AddCustomRoute(string route, string name, string icon)
        {
            _customRoutes.Add(new CustomRoute(route, name, icon));

            return this;
        }
    }
}
