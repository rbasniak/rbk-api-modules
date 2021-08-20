namespace rbkApiModules.Utilities.Charts.ChartJs
{
    public class TooltipBuilder<TFactory, TChart> where TChart : BaseChart where TFactory : BaseChartBuilder<TFactory, TChart>
    {
        internal BaseChartBuilder<TFactory, TChart> Builder { get; }
        public TFactory Chart => Builder as TFactory;

        public TooltipBuilder(BaseChartBuilder<TFactory, TChart> chartBuilder)
        {
            Builder = chartBuilder;
        }

        public TooltipBuilder<TFactory, TChart> BackgroundColor(string color)
        {
            Chart.Builder.Config.Plugins.Tooltip.BackgroundColor = color;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> UseSeriesPointStyles()
        {
            Chart.Builder.Config.Plugins.Tooltip.UsePointStyle = true;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> CornerRadius(double radius)
        {
            Chart.Builder.Config.Plugins.Tooltip.CornerRadius = radius;

            return this;
        }

        public TooltipBuilder<TFactory, TChart> AtPointLocation()
        {
            Chart.Builder.Config.Plugins.Tooltip.SetTooltipPosition(TooltipPosition.Nearest);

            return this;
        }

        public TooltipBuilder<TFactory, TChart> AtVerticalAxis()
        {
            Chart.Builder.Config.Plugins.Tooltip.SetTooltipPosition(TooltipPosition.Nearest);
            Chart.Builder.Config.Interaction.Mode = "index";
            Chart.Builder.Config.Plugins.Tooltip.SetMode(TooltipMode.Index);

            return this;
        }

        public TooltipBuilder<TFactory, TChart> Border(string color, double width)
        {
            Chart.Builder.Config.Plugins.Tooltip.BorderColor = color;
            Chart.Builder.Config.Plugins.Tooltip.BorderWidth = width;

            return this;
        }
    }
}