namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TitleBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        public BaseChartBuilder<TFactory, TChart> Chart { get; }

        public TitleBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Chart = chartBuilder;
        }

        public TitleBuilder<TFactory, TChart> Padding(double top, double bottom)
        {
            Chart.Builder.Config.Plugins.Title.Padding = new PaddingOptions
            {
                Bottom = bottom,
                Top = top
            };

            return this;
        }

        public TitleBuilder<TFactory, TChart> Font(double size, double? lineHeight = null, string family = null)
        {
            Chart.Builder.Config.Plugins.Title.Font = new FontOptions
            {
                Size = size,
                LineHeight = lineHeight,
                Family = family
            };

            return this;
        }

        public TitleBuilder<TFactory, TChart> Alignment(AlignmentType alignment)
        {
            Chart.Builder.Config.Plugins.Title.Align = alignment;

            return this;
        }

        public TitleBuilder<TFactory, TChart> At(PositionType position)
        {
            Chart.Builder.Config.Plugins.Title.Position = position;

            return this;
        }

        public TitleBuilder<TFactory, TChart> Color(string color)
        {
            Chart.Builder.Config.Plugins.Title.Color = color;

            return this;
        }
    }
}