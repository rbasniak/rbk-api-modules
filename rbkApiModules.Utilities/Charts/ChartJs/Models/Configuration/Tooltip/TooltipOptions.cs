namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TooltipOptions
    {
        public string BackgroundColor { get; internal set; }
        public bool? Intersect { get; internal set; }
        public bool? Enabled { get; internal set; }
        public bool? UsePointStyle { get; internal set; }
        public double? TitleSpacing{ get; internal set; }
        public double? TitleMarginBottom { get; internal set; }
        public string TitleFont { get; internal set; }
        public string TitleColor { get; internal set; }
        public string TitleAlign { get; internal set; }
        public string TextDirection { get; internal set; }
        public bool? Rtl { get; internal set; }
        public string Position { get; internal set; }
        public double? Padding { get; internal set; }
        public string MultiKeyBackground { get; internal set; }
        public string Mode { get; internal set; }
        public double? FooterSpacing { get; internal set; }
        public double? FooterMarginBottom { get; internal set; }
        public string FooterFont { get; internal set; }
        public string FooterColor { get; internal set; }
        public string FooterAlign { get; internal set; }
        public bool? DisplayColors { get; internal set; }
        public double? CornerRadius { get; internal set; }
        public double? CaretSize { get; internal set; }     
        public double? CaretPadding { get; internal set; } 
        public string BorderColor { get; internal set; }
        public double? BorderWidth { get; internal set; }
        public double? BodySpacing { get; internal set; }
        public string BodyFont { get; internal set; }
        public string BodyColor { get; internal set; }
        public string BodyAlign { get; internal set; }

        public void SetMode(TooltipMode mode)
        {
            Mode = mode.ToString().ToLower();
        }

        public void SetBodyAlign(TextAlign align)
        {
            BodyAlign = align.ToString().ToLower();
        }

        public void SetTitleAlign(TextAlign align)
        {
            TitleAlign = align.ToString().ToLower();
        }

        public void SetFooterAlign(TextAlign align)
        {
            FooterAlign = align.ToString().ToLower();
        }

        public void SetTooltipPosition(TooltipPosition position)
        {
            Position = position.ToString().ToLower();
        }

        public void SetTextDirection(TextDirection direction)
        {
            TextDirection = direction.ToString().ToLower();
        }
    }

}
