using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TooltipOptions
    {
        public bool? Enabled { get; set; }
        public string Mode { get; set; }
        public bool? Intersect { get; set; }
        public string BackgroundColor { get; set; }
        public string TitleFontFamily { get; set; }
        public double? TitleFontSize { get; set; }
        public string TitleFontStyle { get; set; }
        public string TitleFontColor { get; set; }
        public double? TitleSpacing { get; set; }
        public double? TitleMarginBottom { get; set; }
        public string BodyFontFamily { get; set; }
        public double? BodyFontSize { get; set; }
        public string BodyFontStyle { get; set; }
        public string BodyFontColor { get; set; }
        public double? BodySpacing { get; set; }
        public string FooterFontFamily { get; set; }
        public double? FooterFontSize { get; set; }
        public string FooterFontStyle { get; set; }
        public string FooterFontColor { get; set; }
        public double? FooterSpacing { get; set; }
        public double? FooterMarginTop { get; set; }
        public double? XPadding { get; set; }
        public double? YPadding { get; set; }
        public double? CaretSize { get; set; }
        public double? CornerRadius { get; set; }
        public string MultiKeyBackground { get; set; }
        public TooltipPosition? Position { get; set; }
        public double? CaretPadding { get; set; }
        public bool? DisplayColors { get; set; }
        public string BorderColor { get; set; }
        public double? BorderWidth { get; set; }
    }
}
