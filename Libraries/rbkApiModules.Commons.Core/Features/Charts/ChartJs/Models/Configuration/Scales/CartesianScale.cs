namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class CartesianScale
    {
        public IndexAxis? Axis { get; internal set; }
        public bool? Display { get; internal set; }
        public GridLineOptions Grid { get; internal set; }
        public double? Min { get; internal set; }
        public double? Max { get; internal set; }
        public bool? Offset { get; internal set; }
        public string Position { get; internal set; }
        public bool? Reverse { get; internal set; }
        public bool? Stacked { get; internal set; } 
        public TickOptions Ticks { get; internal set; }
        public ScaleTitleOptions Title { get; internal set; }

        public bool? BeginAtZero { get; internal set; }
        public double? SuggestedMax { get; internal set; }

        internal void SetPosition(AxisPosition position)
        {
            Position = position.ToString().ToLower();
        }

        public double? SuggestedMin { get; internal set; }

    }
}
