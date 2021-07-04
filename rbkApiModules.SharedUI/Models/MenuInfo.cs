using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.SharedUI
{
    public class MenuInfo
    {
        public bool UseAnalytics { get; set; }
        public bool UseDiagnostics { get; set; }
        public CustomRoute[] CustomRoutes { get; set; }
    }

    public class CustomRoute
    {
        public CustomRoute(string route, string name, string icon)
        {
            Route = route;
            Name = name;
            Icon = icon;
        }

        public string Route { get; set; }
        public string Name { get; set; }
        public string Icon { get; set; }
    }
}
