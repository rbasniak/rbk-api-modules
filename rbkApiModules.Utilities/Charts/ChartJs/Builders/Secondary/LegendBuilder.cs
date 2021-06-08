namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class LegendBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public BaseChartBuilder<TFactory, TChart> Chart { get; }

        public LegendBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Chart = chartBuilder;
        }

        public LegendBuilder<TFactory, TChart> At(AlignmentType alignment)
        {
            Chart.Builder.Config.Plugins.Legend.Align = alignment;

            return this;
        }

        public LegendBuilder<TFactory, TChart> At(PositionType position)
        {
            Chart.Builder.Config.Plugins.Legend.Position = position;

            return this;
        }

        public LegendBuilder<TFactory, TChart> WithTitle(string title)
        {
            Chart.Builder.Config.Plugins.Legend.Title = new LegendTitleOptions 
            {
                Display = true,
                Text = title,
            };

            return this;
        }
    }

    public class LegendTitleBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public LegendBuilder<TFactory, TChart> Legend { get; }

        public LegendTitleBuilder(LegendBuilder<TFactory, TChart> builder)
        {
            Legend = builder;
        }

        public LegendTitleBuilder<TFactory, TChart> At(PositionType position)
        {
            Legend.Chart.Builder.Config.Plugins.Legend.Title.Position = position;

            return this;
        }

        public LegendTitleBuilder<TFactory, TChart> Font(double size, double? lineHeight, string font)
        {
            Legend.Chart.Builder.Config.Plugins.Legend.Title.Font = new FontOptions 
            {
                Family = font,
                LineHeight = lineHeight,
                Size = size
            };

            return this;
        }

        public LegendTitleBuilder<TFactory, TChart> Padding(double left, double top, double right, double bottom)
        {
            Legend.Chart.Builder.Config.Plugins.Legend.Title.Padding = new PaddingOptions
            {
                Top = top,
                Left = left,
                Right = right,
                Bottom = bottom
            };

            return this;
        }
    }
}