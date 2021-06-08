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
        public string BackgroundColor { get; set; }
        public bool? Intersect { get; set; }
        public bool? Enabled { get; set; }
        public bool? UsePointStyle { get; set; }
        public double TitleSpacing{ get; set; }
        public double? TitleMarginBottom { get; set; }
        public string TitleFont { get; set; }
        public string TitleColor { get; set; }
        public TextAlign? TitleAlign { get; set; }
        public TextDirection? TextDirection { get; set; }
        public bool? Rtl { get; set; }
        public TooltipPosition? Position { get; set; }
        public double? Padding { get; set; }
        public string MultiKeyBackground { get; set; }
        public TooltipMode? Mode { get; set; }
        public double FooterSpacing { get; set; }
        public double? FooterMarginBottom { get; set; }
        public string FooterFont { get; set; }
        public string FooterColor { get; set; }
        public TextAlign? FooterAlign { get; set; }
        public bool? DisplayColors { get; set; }
        public double? CornerRadius { get; set; }
        public double? CaretSize { get; set; }     
        public double? CaretPadding { get; set; } 
        public string BorderColor { get; set; }
        public double? BorderWidth { get; set; }
        public double? BodySpacing { get; set; }
        public string BodyFont { get; set; }
        public string BodyColor { get; set; }
        public TextAlign? BodyAlign { get; set; }
    }


}
