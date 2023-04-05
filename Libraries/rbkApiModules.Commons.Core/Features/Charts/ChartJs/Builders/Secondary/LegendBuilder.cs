namespace rbkApiModules.Commons.Charts.ChartJs
{
    public class LegendBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        internal BaseChartBuilder<TFactory, TChart> Builder { get; }

        public TFactory Chart => Builder as TFactory;

        public LegendBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Builder = chartBuilder;
        }

        public LegendBuilder<TFactory, TChart> Align(AlignmentType alignment)
        {
            Builder.Builder.Config.Plugins.Legend.SetAlignment(alignment);

            return this;
        }

        public LegendBuilder<TFactory, TChart> At(PositionType position)
        {
            Builder.Builder.Config.Plugins.Legend.SetPosition(position);

            return this;
        }

        public LegendTitleBuilder<TFactory, TChart> Title(string title)
        {
            Builder.Builder.Config.Plugins.Legend.Title = new LegendTitleOptions 
            {
                Display = true,
                Text = title,
            };

            return new LegendTitleBuilder<TFactory, TChart>(this);
        }


        public LegendBuilder<TFactory, TChart> UsePointStyles()
        {
            if (Builder.Builder.Config.Plugins.Legend.Labels == null) Builder.Builder.Config.Plugins.Legend.Labels = new LegendLabelOptions();

            Builder.Builder.Config.Plugins.Legend.Labels.UsePointStyle = true;

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

        public LegendTitleBuilder<TFactory, TChart> Align(AlignmentType alignment)
        {
            Legend.Builder.Builder.Config.Plugins.Legend.Title.SetAlignment(alignment);

            return this;
        }

        public LegendTitleBuilder<TFactory, TChart> Font(double size, double? lineHeight, string font)
        {
            Legend.Builder.Builder.Config.Plugins.Legend.Title.Font = new FontOptions 
            {
                Family = font,
                LineHeight = lineHeight,
                Size = size
            };

            return this;
        }

        public LegendTitleBuilder<TFactory, TChart> Padding(double left, double top, double right, double bottom)
        {
            Legend.Builder.Builder.Config.Plugins.Legend.Title.Padding = new PaddingOptions
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