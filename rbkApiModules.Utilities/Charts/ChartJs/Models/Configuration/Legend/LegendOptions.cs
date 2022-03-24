namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LegendOptions
    {
        public string Align { get; internal set; }
        public bool? Display { get; internal set; }
        public bool? FullWdidth { get; internal set; }
        public LegendLabelOptions Labels { get; internal set; }
        public string Position { get; internal set; }
        public bool? Reverse { get; internal set; }
        public LegendTitleOptions Title { get; internal set; }

        internal void SetAlignment(AlignmentType alignment)
        {
            Align = alignment.ToString().ToLower();
        }

        internal void SetPosition(PositionType position)
        {
            Position = position.ToString().ToLower();
        }
    }
}
