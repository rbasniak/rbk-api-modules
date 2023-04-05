namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class TitleBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        internal BaseChartBuilder<TFactory, TChart> Builder { get; }
        public TFactory Chart => Builder as TFactory;

        public TitleBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Builder = chartBuilder;
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
            Chart.Builder.Config.Plugins.Title.SetAlignmentType(alignment);

            return this;
        }

        public TitleBuilder<TFactory, TChart> At(PositionType position)
        {
            Chart.Builder.Config.Plugins.Title.SetPositionType(position);

            return this;
        }

        public TitleBuilder<TFactory, TChart> Color(string color)
        {
            Chart.Builder.Config.Plugins.Title.Color = color;

            return this;
        }
    }
}