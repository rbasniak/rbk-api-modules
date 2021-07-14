using System.Collections.Generic;

namespace rbkApiModules.Diagnostics.Core
{
    public class DiagnosticsDashboard
    {
        public object TotalErrors { get; set; }

        public object SourceErrorsRadial { get; set; }
        public object SourceErrorsLinear { get; set; }
        public object BrowserErrorsLinear { get; set; }
        public object BrowserErrorsRadial { get; set; }
        public object AreaErrorsLinear { get; set; }
        public object AreaErrorsRadial { get; set; }
        public object LayerErrorsLinear { get; set; }
        public object LayerErrorsRadial { get; set; }
        public object UserErrorsLinear { get; set; }
        public object UserErrorsRadial { get; set; }
        public object DeviceErrorsLinear { get; set; }
        public object DeviceErrorsRadial { get; set; }
        public object OperatingSystemErrorsLinear { get; set; }
        public object OperatingSystemErrorsRadial { get; set; }
    }
}